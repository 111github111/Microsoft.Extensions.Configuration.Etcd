using System;

namespace Microsoft.Extensions.Configuration.Etcd.Models
{
    public class EtcdAuth
    {
        public EtcdAuth(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }


        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
