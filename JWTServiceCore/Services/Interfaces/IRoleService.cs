using JWTServiceCore.Models.DTOs;
using Microsoft.AspNetCore.Identity;

namespace JWTServiceCore.Services.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetAllRolesAsync();
        Task<RoleDto> GetRoleByIdAsync(string roleId);
        Task<RoleDto> GetRoleByNameAsync(string roleName);
        Task<IdentityResult> CreateRoleAsync(string roleName);
        Task<IdentityResult> UpdateRoleAsync(RoleDto roleDto);
        Task<IdentityResult> DeleteRoleAsync(string roleId);
    }
}
