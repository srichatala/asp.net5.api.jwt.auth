using asp.net5.api.jwt.auth.Model;
using asp.net5.api.jwt.auth.Service.Interface;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace asp.net5.api.jwt.auth.Service
{
    public class JwtAuthService : IJwtAuthService
    {
        private readonly byte[] _secret;
        private readonly AppSettings _appSettings;
        public JwtAuthService(IOptions<AppSettings> options)
        {
            _appSettings = options.Value;
            _secret = Encoding.ASCII.GetBytes(options.Value.SecretKey);
        }

        public JwtResponse GenerateTokens(string userName)
        {
            var claims = new[] {
                new Claim(ClaimTypes.Name, userName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var jwtToken = new JwtSecurityToken(
                _appSettings.Issuer,
                _appSettings.Issuer,
                claims,
                expires: DateTime.Now.AddMinutes(1),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(_secret), SecurityAlgorithms.HmacSha256Signature));
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return new JwtResponse
            {
                AccessToken = accessToken
            };
        }
    }
}
