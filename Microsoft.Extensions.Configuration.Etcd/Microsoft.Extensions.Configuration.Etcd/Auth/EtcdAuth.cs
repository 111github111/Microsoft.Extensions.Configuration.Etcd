using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Configuration.Etcd.Auth
{
    public class EtcdAuth
    {
        public EtcdAuth(string userName, string password)
        {
            this.UserName = userName;
            this.Password = password;
        }


        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
