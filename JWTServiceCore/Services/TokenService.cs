using JWTServiceCore.Configurations;
using JWTServiceCore.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTServiceCore.Services
{
    public class TokenService : ITokenService
    {
        #region Ctor

        private readonly AppSetting _appSetting;

        public TokenService(IOptions<AppSetting> appSetting)
        {
            _appSetting = appSetting.Value;
        }

        #endregion

        public string GenerateToken(string userId, IEnumerable<Claim> additionalClaims, IEnumerable<string> roles)
        {
            // Authentication successful so generate JWT token
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_appSetting.Secret!);

            // Add roles to claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userId)
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Add other claims
            claims.AddRange(additionalClaims);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(
                                     new SymmetricSecurityKey(key),
                                         SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
