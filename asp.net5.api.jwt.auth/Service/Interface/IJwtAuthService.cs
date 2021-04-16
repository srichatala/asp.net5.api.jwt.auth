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
        void RemoveExpiredRefreshTokens(DateTime now);
        void RemoveRefreshTokenByUserName(string userName);
        JwtResponse Refresh(string refreshToken, string accessToken, DateTime now);
    }
}
