using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asp.net5.api.jwt.auth.Model
{
    public class AppSettings
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string ConnectionString { get; set; }
        public int ExpireTime { get; set; }
    }
}
