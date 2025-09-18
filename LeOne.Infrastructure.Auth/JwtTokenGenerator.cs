using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LeOne.Application.Common.Interfaces;
using LeOne.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LeOne.Infrastructure.Auth
{
    public sealed class JwtTokenGenerator(IOptions<JwtSettings> options) : IJwtTokenGenerator
    {
        private readonly JwtSettings _settings = options.Value;
        private readonly JwtSecurityTokenHandler _handler = new JwtSecurityTokenHandler
        {
            MapInboundClaims = false
        };
        public (string Token, DateTimeOffset ExpiresAt) GenerateAccessToken(User user)
        {
            var now = DateTimeOffset.UtcNow;
            var expires = now.AddMinutes(_settings.ExpiryMinutes);

            var claims = CreateAccessClaims(user, now);
            var jwt = BuildJwt(claims, expires);

            return (_handler.WriteToken(jwt), expires);
        }

        public (string Token, DateTimeOffset ExpiresAt, string Jti) GenerateRefreshToken(User user)
        {
            var now = DateTimeOffset.UtcNow;
            var expires = now.AddDays(_settings.RefreshTokenExpiryDays);

            var (claims, jti) = CreateRefreshClaims(user, now);
            var jwt = BuildJwt(claims, expires);

            return (_handler.WriteToken(jwt), expires, jti);
        }

        public (Guid? UserId, string? Jti) ValidateRefreshTokenRich(string token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));

            try
            {
                var principal = _handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _settings.Issuer,
                    ValidAudience = _settings.Audience,
                    IssuerSigningKey = key,
                }, out _);

                var tokenUse = principal.FindFirst("token_use")?.Value;
                if (!string.Equals(tokenUse, "refresh", StringComparison.Ordinal))
                    return (null, null);

                var sub = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                    ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var jti = principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                if (string.IsNullOrWhiteSpace(sub) || string.IsNullOrWhiteSpace(jti))
                    return (null, null);

                return (Guid.Parse(sub), jti);
            }
            catch
            {
                return (null, null);
            }
        }

        private JwtSecurityToken BuildJwt(IEnumerable<Claim> claims, DateTimeOffset expiresAt)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: expiresAt.UtcDateTime,
                signingCredentials: creds
            );
        }


        private static IEnumerable<Claim> CreateAccessClaims(User user, DateTimeOffset now) =>
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email.Value),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new Claim("token_use", "access")
        ];

        private static (IEnumerable<Claim> Claims, string Jti) CreateRefreshClaims(User user, DateTimeOffset now)
        {
            var jti = Guid.NewGuid().ToString();
            Claim[] claims =
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, jti),
                new Claim(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim("token_use", "refresh")
            ];
            return (claims, jti);
        }
    }
}
