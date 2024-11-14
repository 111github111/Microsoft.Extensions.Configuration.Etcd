using Microsoft.Extensions.Configuration.Etcd.Models;

namespace Microsoft.Extensions.Configuration.Etcd
{
    public class EtcdConfigurationSource : FileConfigurationSource
    {
        private readonly EtcdOptions _etcdOptions;

        public EtcdConfigurationSource(EtcdOptions etcdOptions)
        {
            _etcdOptions = etcdOptions;
        }

        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));


            EnsureDefaults(builder);
            return new EtcdConfigurationProvider(this, _etcdOptions);
        }
    }
}
