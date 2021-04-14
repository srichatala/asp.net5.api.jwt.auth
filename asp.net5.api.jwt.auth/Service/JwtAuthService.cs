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
            _secret = Encoding.UTF8.GetBytes(options.Value.SecretKey);
        }

        public JwtResponse GenerateTokens(string userName)
        {
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new[] { new Claim("sub", "customer") }),
                Issuer = _appSettings.Issuer,
                Claims = new Dictionary<string, object>
                {
                    ["email"] = userName,
                },
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddMinutes(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_secret), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var serializedToken = tokenHandler.WriteToken(token);

            return new JwtResponse
            {
                AccessToken = serializedToken
            };
        }
    }
}
