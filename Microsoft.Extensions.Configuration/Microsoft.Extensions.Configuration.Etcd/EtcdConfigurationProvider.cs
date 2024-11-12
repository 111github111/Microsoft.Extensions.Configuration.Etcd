using dotnet_etcd;
using Etcd.Microsoft.Extensions.Configuration.Client;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Etcd.Auth;
using static Mvccpb.Event.Types;

namespace Etcd.Microsoft.Extensions.Configuration
{
    public class EtcdConfigurationProvider : ConfigurationProvider
    {
        private readonly object _locker = new object();
        private readonly IEtcdKeyValueClient _client;

        public EtcdConfigurationProvider(string serviceUrl, EtcdAuth etcdAuth, string key)
        {
            var etcdClient = new EtcdClient(serviceUrl, configureChannelOptions: s =>
            {
                // ChannelCredentials.Insecure   // 基础认证, 选择这个
                // ChannelCredentials.SecureSsl  // ssl/tls 认证, 选择这个


                s.Credentials = ChannelCredentials.Insecure;
                // s.UnsafeUseInsecureChannelCallCredentials = true;
            });
            _client = new EtcdKeyValueClient(etcdClient, etcdAuth, key);

            // watch 监视
            _client.Watch(key, OnWatchCallback);
        }

        public override void Load()
        {
            Data = _client.GetAllKeys();
        }



        // public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath)
        // {
        //     throw new NotImplementedException();
        // }

        // public IChangeToken GetReloadToken()
        // {
        //     throw new NotImplementedException();
        // }

        // public void Load()
        // {
        // 
        // }

        // public void Set(string key, string value)
        // {
        //     throw new NotImplementedException();
        // }

        // public bool TryGet(string key, out string value)
        // {
        //     throw new NotImplementedException();
        // }


        private void OnWatchCallback(WatchEvent[] events)
        {
            lock (_locker)
            {
                foreach (WatchEvent item in events)
                {
                    if (item.Type == EventType.Put)
                    {
                        if (Data.ContainsKey(item.Key))
                            Data[item.Key] = item.Value;
                        else
                            Data.Add(item.Key, item.Value);
                    }
                    else
                        Data.Remove(item.Key);
                }
            }
        }
    }
}
