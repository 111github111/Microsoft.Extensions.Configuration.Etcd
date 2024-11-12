using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Etcd.Auth;

namespace Etcd.Microsoft.Extensions.Configuration
{
    public class EtcdConfigurationSource : IConfigurationSource
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

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return new EtcdConfigurationProvider(_serviceUrl, _etcdAuth, _key);
        }
    }
}
