using Microsoft.Extensions.DependencyInjection;
using Mkh.Data.Abstractions.Adapter;

namespace Mkh.Data.Abstractions.Options
{
    /// <summary>
    /// 数据库配置
    /// </summary>
    public class DbOptions
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 数据库提供器
        /// </summary>
        public DbProvider Provider { get; set; }

        /// <summary>
        /// 是否开启日志
        /// </summary>
        public bool Log { get; set; }

        /// <summary>
        /// 数据库版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 表前缀
        /// </summary>
        public string TablePrefix { get; set; }

        /// <summary>
        /// 仓储服务生命周期类型
        /// </summary>
        public ServiceLifetime RepositoryServiceLifetime { get; set; } = ServiceLifetime.Singleton;
    }
}
