using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Etcd.Auth;

namespace Etcd.Microsoft.Extensions.Configuration
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddEtcd(this IConfigurationBuilder builder, string serviceUrl, EtcdAuth etcdAuth, string keyPrefix)
        {
            return AddEtcd(builder, serviceUrl, etcdAuth, [keyPrefix]);
        }

        public static IConfigurationBuilder AddEtcd(this IConfigurationBuilder builder, string serviceUrl, EtcdAuth etcdAuth, string[] keyPrefixs)
        {
            foreach (var keyPrefix in keyPrefixs)
                builder.Add(new EtcdConfigurationSource(serviceUrl, etcdAuth, keyPrefix));
            return builder;
        }
    }
}
