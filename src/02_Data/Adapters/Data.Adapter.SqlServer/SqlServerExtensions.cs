using System;
using Mkh.Data.Abstractions;
using Mkh.Data.Abstractions.Adapter;
using Mkh.Data.Abstractions.Options;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        // <summary>
        /// 使用SqlServer数据库
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="configure">自定义配置</param>
        /// <returns></returns>
        public static IDbBuilder UseSqlServer(this IDbBuilder builder, string connectionString, Action<DbOptions> configure = null)
        {
            builder.UseDb(connectionString, DbProvider.SqlServer, configure);

            return builder;
        }
    }
}
