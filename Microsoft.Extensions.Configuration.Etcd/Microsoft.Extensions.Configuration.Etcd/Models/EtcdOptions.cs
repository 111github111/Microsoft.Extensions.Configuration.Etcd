using System;

namespace Microsoft.Extensions.Configuration.Etcd.Models
{
    /// <summary>
    /// etcd configuration 参数配置类
    /// </summary>
    public class EtcdOptions
    {
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string ServiceUrl { get; set; }

        /// <summary>
        /// 用户认证信息
        /// </summary>
        public EtcdAuth Auth { get; set; }

        /// <summary>
        /// etcd key 值, 如: app1/production.json
        /// </summary>
        public List<string> Keys { get; set; } = new List<string>();
    }
}
