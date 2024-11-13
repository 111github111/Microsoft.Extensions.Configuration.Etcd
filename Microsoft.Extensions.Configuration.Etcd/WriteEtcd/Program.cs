using Newtonsoft.Json;
using WriteEtcd;
using WriteEtcd.Helpers;
using WriteEtcd.Properties;

// WriteEtcd: 写入测试
// TestEtcd : 读取测试

JsonConvert.DefaultSettings = () => JsonHelpers.Settings;


EtcdHttpDemo httpDemo = new EtcdHttpDemo();
EtcdClientDemo clientDemo = new EtcdClientDemo();


clientDemo.By3_GetTokenGetValue();


// httpDemo.By2_GetTokenAndPut("app1/");
httpDemo.By2_GetTokenAndPut("app1/production.json", Resources.production);
httpDemo.By2_GetTokenAndPut("app1/development.json", Resources.development);


while (true)
{
    // httpDemo.By2_GetTokenAndPut();
    // clientDemo.By2_GetTokenAndPut();

    // httpDemo.By3_Watch();
}



Console.WriteLine();
Console.WriteLine("------ end ------");