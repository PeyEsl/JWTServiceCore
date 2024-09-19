using JWTServiceCore.Models.DTOs;
using Microsoft.AspNetCore.Identity;

namespace JWTServiceCore.Services.Interfaces
{
    public interface IClaimService
    {
        Task<IEnumerable<ClaimDto>> GetClaimsRoleAsync(string roleName);
        Task<IEnumerable<ClaimDto>> GetClaimsUserAsync(string userId);
        Task<IdentityResult> CreateClaimForRoleAsync(ClaimDto claimDto, string roleName);
        Task<IdentityResult> CreateClaimForUserAsync(ClaimDto claimDto, string userId);
        Task<IdentityResult> UpdateClaimForRoleAsync(ClaimDto claimDto, string roleName, string claimType);
        Task<IdentityResult> UpdateClaimForUserAsync(ClaimDto claimDto, string userId, string claimType);
        Task<IdentityResult> UpdateClaimsForRoleAsync(IEnumerable<ClaimDto> claimDtos, string roleName);
        Task<IdentityResult> UpdateClaimsForUserAsync(IEnumerable<ClaimDto> claimDtos, string userId);
        Task<IdentityResult> DeleteClaimForRoleAsync(ClaimDto claimDto, string roleName);
        Task<IdentityResult> DeleteClaimForUserAsync(ClaimDto claimDto, string userId);
        Task<IdentityResult> DeleteClaimsForUserAsync(string userId);
    }
}
