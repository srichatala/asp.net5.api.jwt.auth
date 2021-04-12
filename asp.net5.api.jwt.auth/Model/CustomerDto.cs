using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asp.net5.api.jwt.auth.Model
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
    }
}
