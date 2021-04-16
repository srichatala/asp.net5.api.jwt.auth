using asp.net5.api.jwt.auth.Model;
using asp.net5.api.jwt.auth.Service.Interface;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace asp.net5.api.jwt.auth.Service
{
    public class JwtAuthService : IJwtAuthService
    {
        private readonly byte[] _secret;
        private readonly AppSettings _appSettings;
        private readonly ConcurrentDictionary<string, RefreshToken> _usersRefreshTokens;
        public JwtAuthService(IOptions<AppSettings> options)
        {
            _appSettings = options.Value;
            _usersRefreshTokens = new ConcurrentDictionary<string, RefreshToken>();
            _secret = Encoding.UTF8.GetBytes(options.Value.SecretKey);
        }

        public void RemoveExpiredRefreshTokens(DateTime now)
        {
            var expiredTokens = _usersRefreshTokens.Where(x => x.Value.ExpireAt < now).ToList();
            foreach (var expiredToken in expiredTokens)
            {
                _usersRefreshTokens.TryRemove(expiredToken.Key, out _);
            }
        }

        public void RemoveRefreshTokenByUserName(string userName)
        {
            var refreshTokens = _usersRefreshTokens.Where(x => x.Value.UserName == userName).ToList();
            foreach (var refreshToken in refreshTokens)
            {
                _usersRefreshTokens.TryRemove(refreshToken.Key, out _);
            }
        }

        public JwtResponse GenerateTokens(string userName)
        {
            var tokenDescriptor = new SecurityTokenDescriptor {
                Issuer = _appSettings.Issuer,
                Audience = _appSettings.Issuer,
                Claims = new Dictionary<string, object>
                {
                    ["Name"] = userName,
                },
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddMinutes(_appSettings.ExpireTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_secret), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var serializedToken = tokenHandler.WriteToken(token);

            var refreshToken = new RefreshToken
            {
                UserName = userName,
                TokenString = GenerateRefreshTokenString(),
                ExpireAt = DateTime.Now.AddMinutes(_appSettings.ExpireTime)
            };
            _usersRefreshTokens.AddOrUpdate(refreshToken.TokenString, refreshToken, (_, _) => refreshToken);

            return new JwtResponse
            {
                AccessToken = serializedToken,
                RefreshToken = refreshToken
            };
        }

        public JwtResponse Refresh(string refreshToken, string accessToken, DateTime now)
        {
            var (principal, jwtToken) = DecodeJwtToken(accessToken);
            if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature))
            {
                throw new SecurityTokenException("Invalid token");
            }

            var userName = principal.Identity?.Name;
            if (!_usersRefreshTokens.TryGetValue(refreshToken, out var existingRefreshToken))
            {
                throw new SecurityTokenException("Invalid token");
            }
            if (existingRefreshToken.UserName != userName || existingRefreshToken.ExpireAt < now)
            {
                throw new SecurityTokenException("Invalid token");
            }

            return GenerateTokens(userName); // need to recover the original claims
        }

        public (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new SecurityTokenException("Invalid token");
            }
            var principal = new JwtSecurityTokenHandler()
                .ValidateToken(token,
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(_secret),
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    },
                    out var validatedToken);
            return (principal, validatedToken as JwtSecurityToken);
        }

        private static string GenerateRefreshTokenString()
        {
            var randomNumber = new byte[32];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
