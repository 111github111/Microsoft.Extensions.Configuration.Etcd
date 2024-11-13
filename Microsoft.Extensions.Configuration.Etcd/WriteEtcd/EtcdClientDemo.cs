using dotnet_etcd;
using Etcdserverpb;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;

namespace WriteEtcd
{
    /// <summary>
    /// 当前 etcd 使用版本：v3.4.26
    /// </summary>
    public class EtcdClientDemo
    {
        /// <summary> version >= 7.0.0 , 需要这个配置</summary>
        private Action<GrpcChannelOptions> grpcOptions = s => s.Credentials = ChannelCredentials.Insecure;

        /// <summary>
        /// 1. 创建 root 用户、角色、授予角色、启用
        /// </summary>
        public void By1_CreateRoot()
        {
            var client = new EtcdClient("http://127.0.0.1:2379", configureChannelOptions: grpcOptions);

            // 创建 root 用户
            {
                var result = client.UserAdd(new AuthUserAddRequest { Name = "root", Password = "123456" });
                Console.WriteLine($"UserAdd: Revision = {result.Header.Revision}，MemberId = {result.Header.MemberId}");
            }


            // 创建 root 角色
            {
                var result = client.RoleAdd(new AuthRoleAddRequest { Name = "root" });
                Console.WriteLine($"RoleAdd: Revision = {result.Header.Revision}，MemberId = {result.Header.MemberId}");
            }


            // 为 root 用户授予角色
            {
                var result = client.UserGrantRole(new AuthUserGrantRoleRequest { User = "root", Role = "root" });
                Console.WriteLine($"UserGrantRole: Revision = {result.Header.Revision}，MemberId = {result.Header.MemberId}");
            }


            // 开启权限
            {
                var result = client.AuthEnable(new AuthEnableRequest { });
                Console.WriteLine($"AuthEnable: Revision = {result.Header.Revision}，MemberId = {result.Header.MemberId}");
            }

        }

        /// <summary>
        /// 2. 获取 token 以及 写入数据
        /// </summary>
        public void By2_GetTokenAndPut()
        {
            // 获取 root 用户的认证令牌，然后执行 put 写入操作
            var etcdClient = new EtcdClient("https://localhost:2379", configureChannelOptions: grpcOptions);
            {
                // 使用 EtcdClient 获取令牌
                var etcdResponse = etcdClient.Authenticate(new AuthenticateRequest { Name = "root", Password = "123456" });
                Console.WriteLine("etcdResponse.Token = " + etcdResponse.Token);

                // 这里使用 authorization or token 都可以
                var metaData1 = new Metadata { new Metadata.Entry("authorization", etcdResponse.Token) };
                var putResult1 = etcdClient.Put("a111", "123456", metaData1);
                Console.WriteLine("Revision = " + putResult1.Header.Revision);

                // 再次写入 put, 数据版本更新
                var metaData2 = new Metadata { new Metadata.Entry("token", etcdResponse.Token) };
                var data = new PutRequest { Key = ByteString.CopyFromUtf8("a111"), Value = ByteString.CopyFromUtf8("new123456") };
                var putResult = etcdClient.Put(data, metaData2);
                Console.WriteLine("Revision = " + putResult.Header.Revision);
            }
        }

        /// <summary>
        /// 3. 获取值
        /// </summary>
        public void By3_GetTokenGetValue()
        {
            var client = new EtcdClient("http://127.0.0.1:2379", configureChannelOptions: grpcOptions);
            var etcdResponse = client.Authenticate(new AuthenticateRequest { Name = "root", Password = "123456" });
            var metaToken = new Metadata { new Metadata.Entry("token", etcdResponse.Token) };

            {
                var sssss = client.GetRange("", metaToken).Kvs.ToList();



                var kvs = client.Get(new RangeRequest { Key = ByteString.CopyFromUtf8("a111") }, metaToken).Kvs.ToList();
                Console.WriteLine("kvs.Count = " + kvs.Count);
                Console.WriteLine();
            }
        }

        /// <summary>
        /// 4. 获取 token 及使用，添加 watch 观察数据变化
        /// </summary>
        public void By4_GetTokenAndWatch()
        {
            var etcdClient = new EtcdClient("https://localhost:2379", configureChannelOptions: grpcOptions);
            var etcdResponse = etcdClient.Authenticate(new AuthenticateRequest { Name = "root", Password = "123456" });
            var metaData1 = new Metadata { new Metadata.Entry("authorization", etcdResponse.Token) };

            // Watch 是阻塞操作，如果后边还需要执行其他操作的话，需要使用异步的 WatchAsync 方法
            var req = new WatchRequest { CreateRequest = new WatchCreateRequest { Key = ByteString.CopyFromUtf8("a222") } };
            var watchTask = etcdClient.WatchAsync(req, WatchResponseAction, metaData1);


            void WatchResponseAction(WatchResponse watch)
            {
                try
                {
                    if (watch.Events.Count == 0)
                    {
                        Console.WriteLine("watch = ");
                        Console.WriteLine(watch);
                    }
                    else
                    {
                        var kv = watch.Events[0].Kv;
                        Console.WriteLine($"key = {kv.Key.ToStringUtf8()}, value = {kv.Value.ToStringUtf8()}, createRevision = {kv.CreateRevision}, version = {kv.Version}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("异常：" + ex.Message);
                }
            }

            // 使用线程循环写入, 测试 watch
            Task.Run(() =>
            {
                while (true)
                {
                    etcdClient.Put("a222", DateTime.Now.Ticks.ToString(), metaData1);
                    Thread.Sleep(500);
                }
            });
        }
    }
}
