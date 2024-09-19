using System.Security.Claims;

namespace JWTServiceCore.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(string userId, IEnumerable<Claim> additionalClaims, IEnumerable<string> roles);
    }
}
