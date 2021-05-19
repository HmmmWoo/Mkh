using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mkh.Data.Abstractions;
using Mkh.Data.Abstractions.Adapter;
using Mkh.Data.Abstractions.Logger;
using Mkh.Data.Abstractions.Options;
using Mkh.Data.Abstractions.Schema;
using Mkh.Data.Core.Descriptors;
using Mkh.Data.Core.Internal;

namespace Mkh.Data.Core
{
    internal class DbBuilder : IDbBuilder
    {
        private readonly IList<Assembly> _repositoryAssemblies = new List<Assembly>();
        private readonly List<Action> _actions = new List<Action>();
        private readonly Type _dbContextType;

        public IServiceCollection Services { get; set; }

        public DbOptions Options { get; set; }

        public CodeFirstOptions CodeFirstOptions { get; set; }

        public IDbContext DbContext { get; set; }

        public DbBuilder(IServiceCollection services, DbOptions options, Type dbContextType)
        {
            Services = services;
            Options = options;
            _dbContextType = dbContextType;
        }

        public IDbBuilder AddRepositoriesFromAssembly(Assembly assembly)
        {
            if (assembly == null)
                return this;

            _repositoryAssemblies.Add(assembly);
            return this;
        }

        public IDbBuilder AddAction(Action action)
        {
            if (action == null)
                return this;

            _actions.Add(action);
            return this;
        }

        public void Build()
        {
            //创建数据库上下文
            CreateDbContext();

            //加载仓储
            LoadRepositories();

            //执行自定义委托
            foreach (var action in _actions)
            {
                action.Invoke();
            }
        }

        #region ==私有方法==

        /// <summary>
        /// 创建数据库上下文
        /// </summary>
        private void CreateDbContext()
        {
            var sp = Services.BuildServiceProvider();
            var dbLogger = new DbLogger(Options, sp.GetService<IDbLoggerProvider>());
            var accountResolver = sp.GetService<IAccountResolver>();

            //获取数据库适配器的程序集
            var dbAdapterAssemblyName = Assembly.GetCallingAssembly().GetName().Name!.Replace("Core", "Adapter.") + Options.Provider;
            var dbAdapterAssembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(dbAdapterAssemblyName));

            //创建数据库上下文实例，通过反射设置属性
            DbContext = (IDbContext)Activator.CreateInstance(_dbContextType);
            _dbContextType.GetProperty("Options")?.SetValue(DbContext, Options);
            _dbContextType.GetProperty("Logger")?.SetValue(DbContext, dbLogger);
            _dbContextType.GetProperty("Adapter")?.SetValue(DbContext, CreateDbAdapter(dbAdapterAssemblyName, dbAdapterAssembly));
            _dbContextType.GetProperty("SchemaProvider")?.SetValue(DbContext, CreateSchemaProvider(dbAdapterAssemblyName, dbAdapterAssembly));
            _dbContextType.GetProperty("CodeFirstProvider")?.SetValue(DbContext, CreateCodeFirstProvider(dbAdapterAssemblyName, dbAdapterAssembly));
            _dbContextType.GetProperty("AccountResolver")?.SetValue(DbContext, accountResolver);

            // ReSharper disable once AssignNullToNotNullAttribute
            Services.AddSingleton(_dbContextType, DbContext);
        }

        /// <summary>
        /// 创建数据库适配器
        /// </summary>
        /// <returns></returns>
        private IDbAdapter CreateDbAdapter(string dbAdapterAssemblyName, Assembly dbAdapterAssembly)
        {
            var dbAdapterType = dbAdapterAssembly.GetType($"{dbAdapterAssemblyName}.{Options.Provider}DbAdapter");

            Check.NotNull(dbAdapterType, $"数据库适配器{dbAdapterAssemblyName}未安装");

            var dbAdapter = (IDbAdapter)Activator.CreateInstance(dbAdapterType!);
            dbAdapterType.GetProperty("Options")!.SetValue(dbAdapter, Options);
            return dbAdapter;
        }

        /// <summary>
        /// 创建数据库架构提供器实例
        /// </summary>
        /// <returns></returns>
        private ISchemaProvider CreateSchemaProvider(string dbAdapterAssemblyName, Assembly dbAdapterAssembly)
        {
            var schemaProviderType = dbAdapterAssembly.GetType($"{dbAdapterAssemblyName}.{Options.Provider}SchemaProvider");

            return (ISchemaProvider)Activator.CreateInstance(schemaProviderType!, Options.ConnectionString);
        }

        /// <summary>
        /// 创建数据库代码优先提供器实例
        /// </summary>
        /// <returns></returns>
        private ICodeFirstProvider CreateCodeFirstProvider(string dbAdapterAssemblyName, Assembly dbAdapterAssembly)
        {
            var schemaProviderType = dbAdapterAssembly.GetType($"{dbAdapterAssemblyName}.{Options.Provider}CodeFirstProvider");

            return (ICodeFirstProvider)Activator.CreateInstance(schemaProviderType!, CodeFirstOptions, DbContext);
        }

        /// <summary>
        /// 加载仓储
        /// </summary>
        private void LoadRepositories()
        {
            if (_repositoryAssemblies.IsNullOrEmpty())
                return;

            foreach (var assembly in _repositoryAssemblies)
            {
                var serviceLifetime = DbContext.Options.RepositoryServiceLifetime;
                if (serviceLifetime == ServiceLifetime.Transient)
                {
                    throw new ArgumentException("仓储暂不支持瞬时模式(Transient)注入");
                }

                var services = Services;
                if (serviceLifetime == ServiceLifetime.Scoped)
                {
                    //尝试添加仓储实例管理器
                    services.TryAddScoped<IRepositoryManager, RepositoryManager>();
                }

                var repositoryTypes = assembly.GetTypes().Where(m => !m.IsInterface && typeof(IRepository).IsImplementType(m)).ToList();
                if (repositoryTypes.Any())
                {
                    foreach (var type in repositoryTypes)
                    {
                        //按照框架约定，仓储的第二个接口类型就是所需的仓储接口
                        var interfaceType = type.GetInterfaces()[2];

                        //按照约定，仓储接口的第一个接口的泛型参数即为对应实体类型
                        var entityType = interfaceType.GetInterfaces()[0].GetGenericArguments()[0];
                        //保存实体描述符
                        DbContext.EntityDescriptors.Add(new EntityDescriptor(DbContext, entityType));

                        if (serviceLifetime == ServiceLifetime.Scoped)
                        {
                            Services.AddScoped(interfaceType, sp =>
                            {
                                var instance = Activator.CreateInstance(type);
                                var initMethod = type.GetMethod("Init", BindingFlags.Instance | BindingFlags.NonPublic);
                                initMethod!.Invoke(instance, new Object[] { DbContext });

                                //保存仓储实例
                                var manager = sp.GetService<IRepositoryManager>();
                                manager?.Add((IRepository)instance);

                                return instance;
                            });
                        }
                        else
                        {
                            var instance = Activator.CreateInstance(type);
                            var initMethod = type.GetMethod("Init", BindingFlags.Instance | BindingFlags.NonPublic);
                            initMethod!.Invoke(instance, new Object[] { DbContext });
                            Services.AddSingleton(interfaceType, instance!);
                        }
                        //保存仓储描述符
                        DbContext.RepositoryDescriptors.Add(new RepositoryDescriptor(interfaceType, type));
                    }
                }
            }
        }

        #endregion
    }
}
