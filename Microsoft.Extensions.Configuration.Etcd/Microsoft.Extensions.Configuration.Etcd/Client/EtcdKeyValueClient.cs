using dotnet_etcd;
using dotnet_etcd.interfaces;
using Etcdserverpb;
using Grpc.Core;
using Microsoft.Extensions.Configuration.Etcd.Auth;
using Microsoft.Extensions.Configuration.Etcd.Helpers;
using System.Text.Json;

namespace Etcd.Microsoft.Extensions.Configuration.Client
{
    public class EtcdKeyValueClient : IEtcdKeyValueClient
    {
        private readonly IEtcdClient _client;
        private readonly EtcdAuth _etcdAuth;

        private string _token = null;
        private DateTime _recordTime = DateTime.Now;
        private TimeSpan _delayTime = TimeSpan.FromMinutes(5);


        public EtcdKeyValueClient(IEtcdClient client, EtcdAuth etcdAuth)
        {
            _etcdAuth = etcdAuth;
            _client = client;

            CheckIsAuthenticated(true);
        }


        public Dictionary<string, string?> GetAllKeys(string key)
        {

            var dic = new Dictionary<string, string>();

            // 加载 etcd 上 指定的 key-value 项目
            var strJson = _client.GetVal(key, GetMetadata());
            var values = JsonHelpers.ExtractValues(JsonDocument.Parse(strJson).RootElement, string.Empty);
            foreach (var item in values)
                dic.TryAdd(item.key.TrimStart(':'), item.value);


            // 加载 etcd 上所有的 key-values
            // var kvs = _client.GetRange("", GetMetadata()).Kvs.ToList();
            // foreach (var item in kvs)
            //     dic.TryAdd(item.Key.ToStringUtf8(), item.Value.ToStringUtf8());

            return dic;
        }


        // ------------------------- 工具方法 -------------------------

        public void CheckIsAuthenticated(bool isFirst = false)
        {
            if (_etcdAuth == null || string.IsNullOrWhiteSpace(_etcdAuth.UserName) || string.IsNullOrWhiteSpace(_etcdAuth.Password))
                throw new InvalidOperationException("Etcd client authenticate info is null");


            // 1. 超过5分钟, 将再次刷新凭证
            // 2. token 为空的情况下, 会先执行一次认证操作
            if (_recordTime + _delayTime <= DateTime.Now || isFirst)
            {

                var  authRequest = new AuthenticateRequest { Name = _etcdAuth.UserName, Password = _etcdAuth.Password };
                var response = _client.Authenticate(authRequest);
                _token = response.Token;


                // token 判断
                if (string.IsNullOrWhiteSpace(_token))
                    throw new InvalidOperationException("Etcd client is authenticate fail");


                // 时间重置为当前时间
                _recordTime.AddTicks((DateTime.Now - _recordTime).Ticks);
            }

        }

        public Metadata GetMetadata()
        {
            CheckIsAuthenticated(false);

            return new Metadata 
            {
                // new Metadata.Entry("authorization", _token),
                new Metadata.Entry("token", _token),
            };
        }

        public void Watch(string key, Action<WatchEvent[]> method)
        {
            _client.WatchAsync(key, method, GetMetadata());
        }

    }
}
