using Microsoft.Extensions.Configuration.Etcd;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Etcd.Models;
using TestEtcd.Models;


// WriteEtcd: 写入测试
// TestEtcd : 读取测试


Console.WriteLine("start------------");
Console.WriteLine();


// 尝试使用配置的方式读取
var config = new ConfigurationBuilder()
    .AddEtcd(options =>
    {
        options.ServiceUrl = "http://127.0.0.1:2379";
        options.Auth = new EtcdAuth("root", "123456");
        options.Keys.Add("app1/production.json");
    })
// .AddEtcd("http://127.0.0.1:2379", new EtcdAuth("root", "123456"), "app1/production.json")
.Build();

var myItem = config.GetSection("MyItem").Value;
var myConfig = config.GetSection("MyConfigtion").Get<MyConfigtion>();

Console.WriteLine($"myItem = {myItem}");
Console.WriteLine($"myConfig.SqlServer = {myConfig!.SqlServer}");
Console.WriteLine($"myConfig.Redis = {myConfig!.Redis}");


Console.WriteLine();
Console.WriteLine("end------------");
Thread.Sleep(int.MaxValue);