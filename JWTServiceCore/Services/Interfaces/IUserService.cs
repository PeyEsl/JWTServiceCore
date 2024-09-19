using JWTServiceCore.Models.DTOs;
using Microsoft.AspNetCore.Identity;

namespace JWTServiceCore.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(string userId);
        Task<UserDto> GetUserByUserNameAsync(string userName);
        Task<UserDto> GetUserByEmailAsync(string email);
        Task<UserDto> GetUserByPhoneAsync(string phone);
        Task<IdentityResult> RegisterUserAsync(UserDto userDto);
        Task<IdentityResult> RegisterWithPhoneAsync(RegisterWithPhoneDto registerWithPhoneDto);
        Task<IdentityResult> UpdateUserProfileAsync(UpdateProfileDto updateProfileDto);
        Task<IdentityResult> DeleteUserAsync(string userId);
        Task<IEnumerable<string>> GetUserRolesAsync(string userId);
        Task<IdentityResult> AddRoleToUserAsync(string userId, string roleName);
        Task<IdentityResult> AddRolesToUserAsync(string userId, IEnumerable<string> rolesList);
        Task<IdentityResult> RemoveRolesAsync(string userId, IEnumerable<string> roles);
        Task<bool> IsAnyUserName(string userName);
        Task<bool> IsAnyPhone(string phone);
        Task<bool> IsAnyEmail(string email);
    }
}
