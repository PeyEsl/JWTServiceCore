using JWTServiceCore.Models.DTOs;
using JWTServiceCore.Models.Entities;
using JWTServiceCore.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JWTServiceCore.Services
{
    public class UserService : IUserService
    {
        #region Ctor

        private readonly UserManager<Models.Entities.ApplicationUser> _userManager;

        public UserService(UserManager<Models.Entities.ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        #endregion

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();
                if (users == null || !users.Any())
                    return Enumerable.Empty<UserDto>();

                var userDtos = new List<UserDto>();
                foreach (var user in users)
                {
                    userDtos.Add(new UserDto
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        UserName = user.UserName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                    });
                }

                return userDtos;
            }
            catch
            {
                return Enumerable.Empty<UserDto>();
            }
        }

        public async Task<UserDto> GetUserByIdAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return new UserDto();

                var userDto = new UserDto
                {
                    Id = user!.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                };

                return userDto;
            }
            catch
            {
                return new UserDto();
            }
        }

        public async Task<UserDto> GetUserByUserNameAsync(string userName)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                    return new UserDto();

                var userDto = new UserDto
                {
                    Id = user!.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                };

                return userDto;
            }
            catch
            {
                return new UserDto();
            }
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return new UserDto();

                var userDto = new UserDto
                {
                    Id = user!.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                };

                return userDto;
            }
            catch
            {
                return new UserDto();
            }
        }

        public async Task<UserDto> GetUserByPhoneAsync(string phone)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phone);
                if (user == null)
                    return new UserDto();

                var userDto = new UserDto
                {
                    Id = user!.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                };

                return userDto;
            }
            catch
            {
                return new UserDto();
            }
        }

        public async Task<IdentityResult> RegisterUserAsync(UserDto userDto)
        {
            try
            {
                var user = new ApplicationUser
                {
                    FirstName = userDto.FirstName ?? string.Empty,
                    LastName = userDto.LastName ?? string.Empty,
                    UserName = userDto.Email == userDto.UserName ? userDto.UserName!.Split("@")[0].Replace(".", "") : userDto.UserName,
                    Email = userDto.Email ?? string.Empty,
                    PhoneNumber = userDto.PhoneNumber ?? string.Empty,
                    EmailConfirmed = true,
                };

                if (userDto.Password != null)
                {
                    var result = await _userManager.CreateAsync(user!, userDto.Password!);

                    return result;
                }
                else
                {
                    var result = await _userManager.CreateAsync(user!);

                    return result;
                }
            }
            catch
            {
                return new IdentityResult();
            }
        }

        public async Task<IdentityResult> RegisterWithPhoneAsync(RegisterWithPhoneDto registerWithPhoneDto)
        {
            try
            {
                var user = new ApplicationUser
                {
                    PhoneNumber = registerWithPhoneDto.PhoneNumber,
                    UserName = registerWithPhoneDto.PhoneNumber,
                    Email = registerWithPhoneDto.PhoneNumber + "@newRegister.new"
                };

                var result = await _userManager.CreateAsync(user);

                return result;
            }
            catch
            {
                return new IdentityResult();
            }
        }

        public async Task<IdentityResult> UpdateUserProfileAsync(UpdateProfileDto updateProfileDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(updateProfileDto.Id!);
                if (user == null)
                    return IdentityResult.Failed(new IdentityError { Description = "User not found." });

                user.FirstName = updateProfileDto.FirstName;
                user.LastName = updateProfileDto.LastName;
                user.UserName = updateProfileDto.UserName;
                user.Email = updateProfileDto.Email;
                user.PhoneNumber = updateProfileDto.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);

                return result;
            }
            catch
            {
                return new IdentityResult();
            }
        }

        public async Task<IdentityResult> DeleteUserAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return IdentityResult.Failed(new IdentityError { Description = "User not found." });

                var userRoles = await _userManager.GetRolesAsync(user);

                await _userManager.RemoveFromRolesAsync(user, userRoles);

                var result = await _userManager.DeleteAsync(user);

                return result;
            }
            catch
            {
                return new IdentityResult();
            }
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return new List<string>();

                return await _userManager.GetRolesAsync(user!);
            }
            catch
            {
                return new List<string>();
            }
        }

        public async Task<IdentityResult> AddRoleToUserAsync(string userId, string roleName)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return IdentityResult.Failed(new IdentityError { Description = "User not found." });

                return await _userManager.AddToRoleAsync(user!, roleName);
            }
            catch
            {
                return new IdentityResult();
            }
        }

        public async Task<IdentityResult> AddRolesToUserAsync(string userId, IEnumerable<string> rolesList)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return IdentityResult.Failed(new IdentityError { Description = "User not found." });

                return await _userManager.AddToRolesAsync(user!, rolesList);
            }
            catch
            {
                return new IdentityResult();
            }
        }

        public async Task<IdentityResult> RemoveRolesAsync(string userId, IEnumerable<string> roles)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return IdentityResult.Failed(new IdentityError { Description = "User not found." });

                return await _userManager.RemoveFromRolesAsync(user!, roles);
            }
            catch
            {
                return new IdentityResult();
            }
        }

        public async Task<bool> IsAnyUserName(string userName)
        {
            try
            {
                return await _userManager.Users.AnyAsync(u => u.UserName == userName);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsAnyPhone(string phone)
        {
            try
            {
                return await _userManager.Users.AnyAsync(u => u.PhoneNumber == phone);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsAnyEmail(string email)
        {
            try
            {
                return await _userManager.Users.AnyAsync(u => u.Email == email);
            }
            catch
            {
                return false;
            }
        }
    }
}
