using EtcdBak.Models;
using Google.Protobuf;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Net.WebSockets;
using System.Text;

namespace EtcdBak
{
    /// <summary>
    /// 当前 etcd 使用版本：v3.4.26
    /// </summary>
    /// <remarks>
    /// etcd swagger 文档地址
    /// https://github.com/FuxiongYang/etcd/blob/main/Documentation/dev-guide/apispec/swagger/rpc.swagger.json
    /// </remarks>
    public class EtcdHttpDemo
    {
        /// <summary>
        /// 1. 创建 root 用户、角色、授予角色、启用
        /// </summary>
        public void By1_CreateRoot()
        {
            var client = new HttpClient();

            // 创建 root 用户
            {
                var value = new { name = "root", password = "123456" };
                var jsonCtx = new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, MediaTypeNames.Application.Json);
                var result = client.PostAsync("http://127.0.0.1:2379/v3/auth/user/add", jsonCtx).Result.Content.ReadAsStringAsync().Result;
                Console.WriteLine(result);
            }


            // 创建 root 角色
            {
                var jsonCtx = JsonContent.Create(new { name = "root" });
                var result = client.PostAsync("http://127.0.0.1:2379/v3/auth/role/add", jsonCtx).Result.Content.ReadAsStringAsync().Result;
                Console.WriteLine(result);
            }


            // 为 root 用户授予角色
            {
                var jsonCtx = JsonContent.Create(new { user = "root", role = "root" });
                var result = client.PostAsync("http://127.0.0.1:2379/v3/auth/user/grant", jsonCtx).Result.Content.ReadAsStringAsync().Result;
                Console.WriteLine(result);
            }


            // 开启权限
            {
                var jsonCtx = JsonContent.Create(new { });
                var result = client.PostAsync("http://127.0.0.1:2379/v3/auth/enable", jsonCtx).Result.Content.ReadAsStringAsync().Result;
                Console.WriteLine(result);
            }

        }

        /// <summary>
        /// 2. 获取 token 以及 写入数据
        /// </summary>
        public void By2_GetTokenAndPut(string key, string setValue)
        {
            var client = new HttpClient();

            // 获取 root 用户的认证令牌，然后执行写入操作
            {
                // 1. 使用 HttpClient 获取令牌
                var value = new { name = "root", password = "123456" };
                var jsonCtx1 = JsonContent.Create(value);
                var result = client.PostAsync("http://127.0.0.1:2379/v3/auth/authenticate", jsonCtx1).Result.Content.ReadAsStringAsync().Result;
                var authResult = JsonConvert.DeserializeObject<AuthResult>(result);
                Console.WriteLine(authResult?.Token);


                // 2. 写入数据，附带 token 令牌
                //    (* http方式只能使用 Authorization，不能使用 token，使用 token 会报错)
                var jsonCtx2 = JsonContent.Create(new
                {
                    key = ByteString.FromBase64(Convert.ToBase64String(Encoding.UTF8.GetBytes(key))),
                    value = ByteString.FromBase64(Convert.ToBase64String(Encoding.UTF8.GetBytes(setValue)))
                });
                var newClient = new HttpClient();
                newClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authResult?.Token);
                var putResult1 = newClient.PostAsync("http://127.0.0.1:2379/v3/kv/put", jsonCtx2).Result.Content.ReadAsStringAsync().Result;
                Console.WriteLine("putResult1 = " + putResult1);
            }
        }


        /// <summary>
        /// 3. 获取值
        /// </summary>
        public void By3_GetTokenGetValue()
        {
            var client = new HttpClient() { BaseAddress = new Uri("http://127.0.0.1:2379") };
            var jsonCtx1 = JsonContent.Create(new { name = "root", password = "123456" });
            var result = client.PostAsync("/v3/auth/authenticate", jsonCtx1).Result.Content.ReadAsStringAsync().Result;
            var authResult = JsonConvert.DeserializeObject<AuthResult>(result);
            Console.WriteLine(authResult?.Token);
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authResult?.Token); // 认证token
                var param = new { Key = ByteString.FromBase64(Convert.ToBase64String(Encoding.UTF8.GetBytes("a111"))) };
                var value = client.PostAsJsonAsync("/v3/kv/range", param).Result.Content.ReadAsStringAsync().Result;
                Console.WriteLine("value = " + value);
                Console.WriteLine();
            }
        }


        /// <summary>
        /// 有问题，没弄通
        /// </summary>
        public void By3_Watch()
        {
            var watchClient = new HttpClient();

            var jsonCtx = JsonContent.Create(new { name = "root", password = "123456" });
            var result = watchClient.PostAsync("http://127.0.0.1:2379/v3/auth/authenticate", jsonCtx).Result.Content.ReadAsStringAsync().Result;
            var authResult = JsonConvert.DeserializeObject<AuthResult>(result);
            watchClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authResult?.Token); // 认证token
            watchClient.DefaultRequestHeaders.TryAddWithoutValidation("Connection", "keep-alive");         // Connection:keep-alive 表示链接永久有效
            watchClient.DefaultRequestHeaders.TryAddWithoutValidation("Transfer-Encoding", "chunked");     // 设置传输方式为Chunked (分块传输)


            var watchKeys = JsonContent.Create(new { CreateRequest = new { Key = ByteString.CopyFromUtf8("a222") } });
            // var watchPost = watchClient.PostAsync("http://127.0.0.1:2379/v3/watch", watchKeys);


            ClientWebSocket client = new ClientWebSocket();
            var connect = client.ConnectAsync(new Uri("ws://127.0.0.1:2379/v3/watch"), new CancellationToken(false));

            //Task.Run(() =>
            //{
            //    var bytes = new byte[(int)(1024 * 1024 * 0.5)];
            //    ArraySegment<byte> arraySegments = new ArraySegment<byte>(bytes, 0, bytes.Length);
            //});



            // Task.Run(() =>
            // {
            // 
            //     while (true)
            //     {
            //         if (watchPost.IsCanceled)
            //             break;
            // 
            //         Console.WriteLine("IsCanceled = " + watchPost.IsCanceled);
            //         Thread.Sleep(1500);
            // 
            //         Console.WriteLine("IsSuccessStatusCode = " + watchPost.Result);
            //         Console.WriteLine("IsSuccessStatusCode = " + watchPost.Result);
            // 
            //         // Console.WriteLine("IsSuccessStatusCode = " + watchPost.Result);
            //         // if (watchPost.Result.IsSuccessStatusCode)
            //         // {
            //         // 
            //         //     while (true)
            //         //     {
            //         //         var readAsync = watchPost.Result.Content.ReadAsStringAsync();
            //         //         if (readAsync.IsCanceled)
            //         //             break;
            //         // 
            //         //         var myResult = readAsync.Result;
            //         //         if (myResult == null)
            //         //             continue;
            //         // 
            //         //         Console.WriteLine("myResult = " + myResult);
            //         //     }
            //         // }
            //     }
            // });


            var writeClient = new HttpClient();
            writeClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authResult?.Token); // 认证token
            var byteKey = ByteString.FromBase64(Convert.ToBase64String(Encoding.UTF8.GetBytes("a222")));
            while (true)
            {
                var byteValue = ByteString.FromBase64(Convert.ToBase64String(Encoding.UTF8.GetBytes(DateTime.Now.Ticks.ToString())));
                var wrCtx = JsonContent.Create(new { key = byteKey, value = byteValue });
                var putResult = writeClient.PostAsync("http://127.0.0.1:2379/v3/kv/put", wrCtx).Result.Content.ReadAsStringAsync().Result;
                Console.WriteLine("put:" + putResult);
                Thread.Sleep(1000);
            }
        }
    }

    public class EtcdServerpbWatchRequest
    {
        public EtcdServerpbWatchCancelRequest cancel_request { get; set; }
        public EtcdServerpbWatchCreateRequest create_request { get; set; }
        public EtcdServerpbWatchProgressRequest progress_request { get; set; }
    }

    public class EtcdServerpbWatchCancelRequest
    {
        public string watch_id { get; set; }
    }
    public class EtcdServerpbWatchCreateRequest
    {
        public WatchCreateRequestFilterType filters { get; set; }
        public bool fragment { get; set; }
        public string key { get; set; }
        public bool prev_kv { get; set; }
        public bool progress_notify { get; set; }
        public string range_end { get; set; }
        public string start_revision { get; set; }
        public string watch_id { get; set; }

    }
    public class EtcdServerpbWatchProgressRequest { }


    public enum WatchCreateRequestFilterType
    {
        NOPUT,
        NODELETE
    }
}
