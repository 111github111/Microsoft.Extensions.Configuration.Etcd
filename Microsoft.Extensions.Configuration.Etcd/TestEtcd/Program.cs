using Microsoft.Extensions.Configuration.Etcd;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Etcd.Auth;
using TestEtcd.Models;


// WriteEtcd: 写入测试
// TestEtcd : 读取测试


Console.WriteLine("start------------");


// 尝试使用配置的方式读取
var config = new ConfigurationBuilder()
.AddEtcd("http://127.0.0.1:2379", new EtcdAuth("root", "123456"), "app1/production.json")
.Build();

var myItem = config.GetSection("MyItem").Value;
var myConfig = config.GetSection("MyConfigtion").Get<MyConfigtion>();

Console.WriteLine();



    Console.WriteLine("end------------");