using dotnet_etcd;
using Grpc.Core;

namespace Etcd.Microsoft.Extensions.Configuration.Client
{
    public interface IEtcdKeyValueClient
    {
        /// <summary>获取所有 key-value</summary>
        public Dictionary<string, string?> GetAllKeys(string key);

        public void CheckIsAuthenticated(bool isFirst = false);

        public Metadata GetMetadata();

        public void Watch(string key, Action<WatchEvent[]> method);
    }
}
