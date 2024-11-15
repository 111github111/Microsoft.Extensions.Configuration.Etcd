﻿using dotnet_etcd;
using Grpc.Core;

namespace Microsoft.Extensions.Configuration.Etcd.Client
{
    public interface IEtcdKeyValueClient
    {
        /// <summary>获取所有 key-value</summary>
        public IDictionary<string, string> GetAllKeys(string key);

        public void CheckIsAuthenticated(bool isFirst = false);

        public Metadata GetMetadata();

        public void Watch(string key, Action<WatchEvent[]> method);
    }
}
