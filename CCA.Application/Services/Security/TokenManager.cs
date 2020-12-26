using CCA.Application.Configurations;
using CCA.Models.Storage;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CCA.Application.Services.Security
{
    public static class TokenManager
    {
        private static readonly string _idClaim = "Id";
        private static readonly string _hashClaim = "Hash";
        private static readonly string _emailClaim = "Email";
        private static readonly JwtConfiguration _jwtConfiguration = new JwtConfiguration();
        private static readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

        public static LoginToken GenerateJSONWebToken(User user)
        {
            var secretBytes = Encoding.UTF8.GetBytes(_jwtConfiguration.SecretKey);
            var securityKey = new SymmetricSecurityKey(secretBytes);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var refreshToken = GenerateRefreshToken();
            var claims = new Claim[]
            {
                new Claim(_idClaim, user.RowKey),
                new Claim(_emailClaim, user.NormalizedEmail),
                new Claim(_hashClaim, refreshToken)
            };

            var securityToken = new JwtSecurityToken(
                _jwtConfiguration.Issuer,
                _jwtConfiguration.Audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(_jwtConfiguration.Expires),
                signingCredentials: credentials);

            return new LoginToken(
                _jwtSecurityTokenHandler.WriteToken(securityToken),
                refreshToken);
        }

        public static bool ValidateJSONWebToken(string token, bool validateLifetime, out string id, out string email, out string refreshToken)
        {
            if (string.IsNullOrEmpty(token))
            {
                id = string.Empty;
                email = string.Empty;
                refreshToken = string.Empty;
                return false;
            }

            try
            {
                if (token.StartsWith("Bearer "))
                {
                    token = token.Replace("Bearer ", string.Empty);
                }

                var secretBytes = Encoding.UTF8.GetBytes(_jwtConfiguration.SecretKey);
                var securityKey = new SymmetricSecurityKey(secretBytes);
                var validationParameters = new TokenValidationParameters()
                {
                    ValidateLifetime = validateLifetime,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    RequireAudience = true,
                    RequireSignedTokens = true,
                    RequireExpirationTime = validateLifetime,
                    ValidAlgorithms = new string[] { SecurityAlgorithms.HmacSha256 },
                    ValidIssuer = _jwtConfiguration.Issuer,
                    ValidAudience = _jwtConfiguration.Audience,
                    IssuerSigningKey = securityKey
                };

                var principal = _jwtSecurityTokenHandler.ValidateToken(token, validationParameters, out var securityToken);

                var emailClaim = principal.Claims.FirstOrDefault(c => c.Type == _emailClaim);
                var idClaim = principal.Claims.FirstOrDefault(c => c.Type == _idClaim);
                var refreshTokenClaim = principal.Claims.FirstOrDefault(c => c.Type == _hashClaim);

                if (emailClaim is null || idClaim is null || refreshTokenClaim is null)
                {
                    id = string.Empty;
                    email = string.Empty;
                    refreshToken = string.Empty;
                    return false;
                }

                id = idClaim.Value;
                email = emailClaim.Value;
                refreshToken = refreshTokenClaim.Value;
                return true;
            }
            catch (Exception)
            {
                id = string.Empty;
                email = string.Empty;
                refreshToken = string.Empty;
                return false;
            }
        }

        private static string GenerateRefreshToken()
        {
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            var bytes = new byte[32];
            randomNumberGenerator.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
