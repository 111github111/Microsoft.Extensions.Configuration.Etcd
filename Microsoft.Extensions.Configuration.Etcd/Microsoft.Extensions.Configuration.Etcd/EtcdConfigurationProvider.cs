using dotnet_etcd;
using Microsoft.Extensions.Configuration.Etcd.Client;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Etcd.Auth;
using Microsoft.Extensions.Configuration.Etcd.Helpers;
using System.Text.Json;
using static Mvccpb.Event.Types;
using Google.Type;

namespace Microsoft.Extensions.Configuration.Etcd
{
    public class EtcdConfigurationProvider : FileConfigurationProvider
    {
        private readonly object _locker = new object();

        private readonly IEtcdKeyValueClient _client;
        private readonly string _key;



        public EtcdConfigurationProvider(EtcdConfigurationSource source, string serviceUrl, EtcdAuth etcdAuth, string key) : base(source)
        {


            var etcdClient = new EtcdClient(serviceUrl, configureChannelOptions: s =>
            {
                // ChannelCredentials.Insecure   // 基础认证, 选择这个
                // ChannelCredentials.SecureSsl  // ssl/tls 认证, 选择这个
                s.Credentials = ChannelCredentials.Insecure;
                // s.UnsafeUseInsecureChannelCallCredentials = true;
            });


            _client = new EtcdKeyValueClient(etcdClient, etcdAuth);
            _key = key;

            // watch 监视
            _client.Watch(key, OnWatchCallback);
        }

        /// <summary>默认加载 EtcdClient 请求的数据</summary>
        public override void Load()
        {
            Data = _client.GetAllKeys(_key);
        }

        /// <summary>加载 stream 参数作为数据源</summary>
        public override void Load(Stream stream)
        {
            using var reader = new StreamReader(stream);
            Data = JsonHelpers.ExtractValues(reader.ReadToEnd());
        }

        private void OnWatchCallback(WatchEvent[] events)
        {
            lock (_locker)
            {
                foreach (WatchEvent watch in events)
                {
                    if (watch.Type == EventType.Put)
                    {
                        var keyValues = JsonHelpers.ExtractValues(watch.Value); // json 转 IDictionary<,>


                        var oldKeys = Data?.Keys.ToList() ?? new List<string>(); // 更新前的 keys
                        var newKeys = keyValues.Keys.ToList();                   // 更新后的 keys

                        foreach (var oldKey in oldKeys)
                        {
                            // 1. 查找被移除的 key, 并移除
                            if (!newKeys.Any(s => s == oldKey))
                                Data.Remove(oldKey);
                        }


                        foreach (var item in keyValues)
                        {
                            if (Data.ContainsKey(watch.Key))
                                Data[item.Key] = item.Value;    // 2. 之前已存在的 key, 更新
                            else
                                Data.Add(item.Key, item.Value); // 3. 之前不存在的 key, 添加
                        }
                    }
                    else
                    {
                        // 表示当前 json 配置文件已被移除
                        Data = new Dictionary<string, string>();
                    }
                }
            }
        }
    }
}
