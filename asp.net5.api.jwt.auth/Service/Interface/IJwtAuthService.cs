using asp.net5.api.jwt.auth.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asp.net5.api.jwt.auth.Service.Interface
{
    public interface IJwtAuthService
    {
        JwtResponse GenerateTokens(string userName);
    }
}
