# Microsoft.Extensions.Configuration.Etcd


``` 
appsettings.json 

"JwtConfigOptions": {
  "Issuer": "xxx",
  "Audience": "xxx",
  "SecurityKey": "xxxxxxxx5oxxxx5xxxxxszkxxxxdlxxxbxxx"
}
```

example:
```
builder.Configuration
       .AddEtcd(new Credentials("root", "123456"), new EtcdSettings("http://127.0.0.1:2379"), "app1/production.json")
       .Build();
```

use:
```
    private readonly IConfiguration _configuration;
    public MyConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public JwtConfigOptions JwtConfig => _configuration.GetSection(nameof(JwtConfigOptions)).Get<JwtConfigOptions>();
```
