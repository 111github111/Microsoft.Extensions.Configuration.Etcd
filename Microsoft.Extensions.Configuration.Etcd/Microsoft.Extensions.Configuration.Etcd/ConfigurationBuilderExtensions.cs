using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Etcd.Models;

namespace Microsoft.Extensions.Configuration.Etcd
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddEtcd(this IConfigurationBuilder builder, string serviceUrl, EtcdAuth etcdAuth, string keyPrefix)
        {
            return AddEtcd(builder, options =>
            {
                options.ServiceUrl = serviceUrl;
                options.Auth = etcdAuth;
                options.Keys = string.IsNullOrWhiteSpace(keyPrefix) ? new List<string>() : new List<string>() { keyPrefix };
            });
        }

        public static IConfigurationBuilder AddEtcd(this IConfigurationBuilder builder, string serviceUrl, EtcdAuth etcdAuth, string[] keyPrefixs)
        {
            return AddEtcd(builder, options =>
            {
                options.ServiceUrl = serviceUrl;
                options.Auth = etcdAuth;
                options.Keys = keyPrefixs?.ToList() ?? new List<string>();
            });
        }

        public static IConfigurationBuilder AddEtcd(this IConfigurationBuilder builder, Action<EtcdOptions> etcdOptions)
        {
            var myParam = new EtcdOptions();
            etcdOptions(myParam);

            foreach (var keyPrefix in myParam.Keys)
            {
                var myOptions = new EtcdOptions
                {
                    ServiceUrl = myParam.ServiceUrl,
                    Auth = myParam.Auth,
                    Keys = new List<string>() { keyPrefix }
                };
                builder.Add(new EtcdConfigurationSource(myOptions));
            }
            return builder;
        }
    }
}
