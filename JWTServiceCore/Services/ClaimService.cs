using JWTServiceCore.Models.DTOs;
using JWTServiceCore.Models.Entities;
using JWTServiceCore.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace JWTServiceCore.Services
{
    public class ClaimService : IClaimService
    {
        #region Ctor

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public ClaimService(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        #endregion

        public async Task<IEnumerable<ClaimDto>> GetClaimsRoleAsync(string roleName)
        {
            try
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null)
                    return Enumerable.Empty<ClaimDto>();

                var claimDtos = new List<ClaimDto>();

                var claims = await _roleManager.GetClaimsAsync(role);

                foreach (var claim in claims)
                {
                    claimDtos.Add(new ClaimDto
                    {
                        Type = claim.Type,
                        Value = claim.Value,
                    });
                }

                return claimDtos;
            }
            catch
            {
                return Enumerable.Empty<ClaimDto>();
            }
        }

        public async Task<IEnumerable<ClaimDto>> GetClaimsUserAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return Enumerable.Empty<ClaimDto>();

                var claimDtos = new List<ClaimDto>();

                var claims = await _userManager.GetClaimsAsync(user);
                if (claims == null)
                    return Enumerable.Empty<ClaimDto>();

                foreach (var claim in claims)
                {
                    claimDtos.Add(new ClaimDto
                    {
                        Type = claim.Type,
                        Value = claim.Value,
                    });
                }

                return claimDtos;
            }
            catch
            {
                return Enumerable.Empty<ClaimDto>();
            }
        }

        public async Task<IdentityResult> CreateClaimForRoleAsync(ClaimDto claimDto, string roleName)
        {
            try
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null)
                    return IdentityResult.Failed(new IdentityError { Description = "Role not found." });

                var claim = new Claim(claimDto.Type!, claimDto.Value!);

                return await _roleManager.AddClaimAsync(role, claim);
            }
            catch
            {
                return new IdentityResult();
            }
        }

        public async Task<IdentityResult> CreateClaimForUserAsync(ClaimDto claimDto, string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return IdentityResult.Failed(new IdentityError { Description = "Role not found." });

                var claim = new Claim(claimDto.Type!, claimDto.Value!);

                return await _userManager.AddClaimAsync(user, claim);
            }
            catch
            {
                return new IdentityResult();
            }
        }

        public async Task<IdentityResult> UpdateClaimForRoleAsync(ClaimDto claimDto, string roleName, string claimType)
        {
            try
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null)
                    return IdentityResult.Failed(new IdentityError { Description = "Role not found." });

                var oldClaims = await _roleManager.GetClaimsAsync(role);

                var newClaim = new Claim(claimDto.Type!, claimDto.Value!);
                if (oldClaims != null)
                {
                    foreach (var oldClaim in oldClaims)
                    {
                        if (oldClaim.Type == claimType)
                        {
                            var removeResult = await _roleManager.RemoveClaimAsync(role, oldClaim);
                            if (!removeResult.Succeeded)
                            {
                                return IdentityResult.Failed(new IdentityError { Description = "Failed to remove claim from role." });
                            }
                        }
                    }

                    var addResult = await _roleManager.AddClaimAsync(role, newClaim);
                    if (!addResult.Succeeded)
                        return IdentityResult.Failed(new IdentityError { Description = "Failed to add claim to role." });
                }

                return IdentityResult.Success;
            }
            catch
            {
                return new IdentityResult();
            }
        }

        public async Task<IdentityResult> UpdateClaimForUserAsync(ClaimDto claimDto, string userId, string claimType)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return IdentityResult.Failed(new IdentityError { Description = "User not found." });

                var oldClaims = await _userManager.GetClaimsAsync(user);

                var newClaim = new Claim(claimDto.Type!, claimDto.Value!);
                if (oldClaims != null)
                {
                    foreach (var oldClaim in oldClaims)
                    {
                        if (oldClaim.Type == claimType)
                        {
                            var removeResult = await _userManager.RemoveClaimAsync(user, oldClaim);
                            if (!removeResult.Succeeded)
                            {
                                return IdentityResult.Failed(new IdentityError { Description = "Failed to remove claim from user." });
                            }
                        }
                    }

                    var addResult = await _userManager.AddClaimAsync(user, newClaim);
                    if (!addResult.Succeeded)
                        return IdentityResult.Failed(new IdentityError { Description = "Failed to add claim to role." });
                }

                return IdentityResult.Success;
            }
            catch
            {
                return new IdentityResult();
            }
        }

        public async Task<IdentityResult> UpdateClaimsForRoleAsync(IEnumerable<ClaimDto> claimDtos, string roleName)
        {
            try
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null)
                    return IdentityResult.Failed(new IdentityError { Description = "Role not found." });

                var oldClaims = await _roleManager.GetClaimsAsync(role);
                if (oldClaims != null)
                {
                    foreach (var oldClaim in oldClaims)
                    {
                        var removeResult = await _roleManager.RemoveClaimAsync(role, oldClaim);
                        if (!removeResult.Succeeded)
                            return IdentityResult.Failed(new IdentityError { Description = "Failed to remove claim from role." });
                    }
                }

                foreach (var claim in claimDtos)
                {
                    var newClaim = new Claim(claim.Type!, claim.Value!);

                    var addResult = await _roleManager.AddClaimAsync(role, newClaim);
                    if (!addResult.Succeeded)
                        return IdentityResult.Failed(new IdentityError { Description = "Failed to add claim to role." });
                }

                return IdentityResult.Success;
            }
            catch
            {
                return new IdentityResult();
            }
        }

        public async Task<IdentityResult> UpdateClaimsForUserAsync(IEnumerable<ClaimDto> claimDtos, string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return IdentityResult.Failed(new IdentityError { Description = "User not found." });

                var oldClaims = await _userManager.GetClaimsAsync(user);
                if (oldClaims != null)
                {
                    var removeResult = await _userManager.RemoveClaimsAsync(user, oldClaims);
                    if (!removeResult.Succeeded)
                        return IdentityResult.Failed(new IdentityError { Description = "Failed to remove claim from user." });
                }

                var newClaims = new List<Claim>();
                foreach (var claim in claimDtos)
                {
                    newClaims.Add(new Claim(claim.Type!, claim.Value!));
                }

                var result = await _userManager.AddClaimsAsync(user, newClaims);

                return result;
            }
            catch
            {
                return new IdentityResult();
            }
        }

        public async Task<IdentityResult> DeleteClaimForRoleAsync(ClaimDto claimDto, string roleName)
        {
            try
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null)
                    return IdentityResult.Failed(new IdentityError { Description = "Role not found." });

                var claims = await _roleManager.GetClaimsAsync(role);
                if (claims != null)
                {
                    foreach (var claim in claims)
                    {
                        var newClaim = new Claim(claimDto.Type!, claimDto.Value!);
                        if (claim.Type == newClaim.Type && claim.Value == newClaim.Value)
                        {
                            var result = await _roleManager.RemoveClaimAsync(role, claim);
                            if (!result.Succeeded)
                                return IdentityResult.Failed(new IdentityError { Description = "Failed to remove claim from role." });
                        }
                    }
                }

                return IdentityResult.Success;
            }
            catch
            {
                return new IdentityResult();
            }
        }

        public async Task<IdentityResult> DeleteClaimForUserAsync(ClaimDto claimDto, string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return IdentityResult.Failed(new IdentityError { Description = "User not found." });

                var claims = await _userManager.GetClaimsAsync(user);
                if (claims != null)
                {
                    foreach (var claim in claims)
                    {
                        var newClaim = new Claim(claimDto.Type!, claimDto.Value!);
                        if (claim.Type == newClaim.Type && claim.Value == newClaim.Value)
                        {
                            var result = await _userManager.RemoveClaimAsync(user, claim);
                            if (!result.Succeeded)
                                return IdentityResult.Failed(new IdentityError { Description = "Failed to remove claim from user." });
                        }
                    }
                }

                return IdentityResult.Success;
            }
            catch
            {
                return new IdentityResult();
            }
        }

        public async Task<IdentityResult> DeleteClaimsForUserAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return IdentityResult.Failed(new IdentityError { Description = "User not found." });

                var claims = await _userManager.GetClaimsAsync(user);
                if (claims != null)
                {
                    var result = await _userManager.RemoveClaimsAsync(user, claims);
                    if (!result.Succeeded)
                        return IdentityResult.Failed(new IdentityError { Description = "Failed to remove claims from user." });
                }

                return IdentityResult.Success;
            }
            catch
            {
                return new IdentityResult();
            }
        }
    }
}
