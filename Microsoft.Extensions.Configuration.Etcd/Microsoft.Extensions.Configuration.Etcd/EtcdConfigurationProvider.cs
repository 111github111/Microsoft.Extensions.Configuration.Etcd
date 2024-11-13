using dotnet_etcd;
using Microsoft.Extensions.Configuration.Etcd.Client;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Etcd.Auth;
using Microsoft.Extensions.Configuration.Etcd.Helpers;
using System.Text.Json;
using static Mvccpb.Event.Types;

namespace Microsoft.Extensions.Configuration.Etcd
{
    public class EtcdConfigurationProvider : FileConfigurationProvider
    {
        private readonly object _locker = new object();

        private readonly JsonDocumentOptions jsonOptions = new JsonDocumentOptions();
        private readonly IEtcdKeyValueClient _client;
        private readonly string _key;



        public EtcdConfigurationProvider(EtcdConfigurationSource source, string serviceUrl, EtcdAuth etcdAuth, string key) : base(source)
        {
            jsonOptions.CommentHandling = JsonCommentHandling.Skip;
            jsonOptions.AllowTrailingCommas = true;


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
            using (var reader = new StreamReader(stream))
            using (JsonDocument doc = JsonDocument.Parse(reader.ReadToEnd(), jsonOptions))
            {
                var dic = new Dictionary<string, string>();
                var values = JsonHelpers.ExtractValues(doc.RootElement, string.Empty);
                foreach (var item in values)
                    dic.TryAdd(item.key.TrimStart(':'), item.value);
                Data = dic;
            }
        }

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
