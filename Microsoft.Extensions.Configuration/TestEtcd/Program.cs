// See https://aka.ms/new-console-template for more information
using Etcd.Microsoft.Extensions.Configuration;
using Etcdserverpb;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Etcd.Auth;

Console.WriteLine("start------------");


// 尝试使用配置的方式读取
var config = new ConfigurationBuilder()
.AddEtcd("http://127.0.0.1:2379", new EtcdAuth("root", "123456"), "app1/production.json")
.Build();



var myItem = config.GetSection("MyItem").Value;
Console.WriteLine();



Console.WriteLine("end------------");