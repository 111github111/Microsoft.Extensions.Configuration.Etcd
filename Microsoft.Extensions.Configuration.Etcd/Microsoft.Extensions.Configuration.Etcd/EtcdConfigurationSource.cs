using Microsoft.Extensions.Configuration.Etcd.Auth;

namespace Microsoft.Extensions.Configuration.Etcd
{
    public class EtcdConfigurationSource : FileConfigurationSource
    {
        private readonly EtcdAuth _etcdAuth;
        private readonly string _serviceUrl;
        private readonly string _key;
        public EtcdConfigurationSource(string serviceUrl, EtcdAuth etcdAuth, string key)
        {
            _serviceUrl = serviceUrl;
            _etcdAuth = etcdAuth;
            _key = key;
        }

        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            EnsureDefaults(builder);
            return new EtcdConfigurationProvider(this, _serviceUrl, _etcdAuth, _key);
        }
    }
}
