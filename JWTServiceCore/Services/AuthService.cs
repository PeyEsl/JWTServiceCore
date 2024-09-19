using JWTServiceCore.Models.DTOs;
using JWTServiceCore.Models.Entities;
using JWTServiceCore.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JWTServiceCore.Services
{
    public class AuthService : IAuthService
    {
        #region Ctor

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthService(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        #endregion

        public async Task<string> GenerateChangeEmailAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return "User not found.";
                }
                else
                {
                    var code = await _userManager.GenerateChangeEmailTokenAsync(user, email);

                    return code;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public async Task<string> GenerateChangePhoneNumberAsync(string phoneNumber)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
                if (user == null)
                {
                    return "User not found.";
                }
                else
                {
                    var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);

                    return code;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public async Task<string> GenerateEmailConfirmationAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return "User not found.";
                }
                else
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    return code;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public async Task<IEnumerable<string>> GenerateNewTwoFactorRecoveryAsync(string userId, int number)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    return new List<string>();
                }
                else
                {
                    var code = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user!, number);

                    return code!;
                }
            }
            catch
            {
                return new List<string>();
            }
        }

        public async Task<string> GeneratePasswordResetAsync(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(resetPasswordDto.UserId!);
                if (user == null)
                {
                    return "User not found.";
                }
                else
                {
                    var code = await _userManager.GeneratePasswordResetTokenAsync(user!);

                    return code;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public async Task<string> GenerateTwoFactorAsync(UserDto userDto, string token)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userDto.Id!);
                if (user == null)
                {
                    return "User not found.";
                }
                else
                {
                    var code = await _userManager.GenerateTwoFactorTokenAsync(user!, token);

                    return code;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public async Task<bool> VerifyTwoFactorAsync(ConfirmDto confirmDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(confirmDto.UserId!);
                if (user == null)
                {
                    return false;
                }
                else
                {
                    var result = await _userManager.VerifyTwoFactorTokenAsync(user!, confirmDto.TokenProvider!, confirmDto.Code!);

                    return result;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<IdentityResult> EmailConfirmAsync(UserDto userDto, string token)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userDto.Email!);
                if (user == null)
                    return IdentityResult.Failed(new IdentityError { Description = "User not found." });

                var result = await _userManager.ConfirmEmailAsync(user, token);

                return result;
            }
            catch
            {
                return new IdentityResult();
            }
        }

        public async Task PhoneNumberConfirmedAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                user!.PhoneNumberConfirmed = true;

                await _userManager.UpdateAsync(user);
            }
            catch
            {
                return;
            }
        }

        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var user = new ApplicationUser();
                if (resetPasswordDto.Email != null)
                    user = await _userManager.FindByEmailAsync(resetPasswordDto.Email!);
                else
                    user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == resetPasswordDto.PhoneNumber);

                if (user == null)
                {
                    return IdentityResult.Failed(new IdentityError { Description = "User not found." });
                }
                else
                {
                    var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token!, resetPasswordDto.NewPassword!);

                    return result;
                }
            }
            catch
            {
                return new IdentityResult();
            }
        }

        public async Task<bool> CheckPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var user = new ApplicationUser();
                if (resetPasswordDto.Email != null)
                    user = await _userManager.FindByEmailAsync(resetPasswordDto.Email!);
                else
                    user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == resetPasswordDto.PhoneNumber);

                if (user == null)
                {
                    return false;
                }
                else
                {
                    var result = await _userManager.CheckPasswordAsync(user, resetPasswordDto.OldPassword!);

                    return result;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<SignInResult> LoginUserAsync(LoginDto loginDto)
        {
            try
            {
                var userByName = await _userManager.FindByNameAsync(loginDto.UserName!);

                var userByPhone = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == loginDto.UserName!);

                var userByEmail = await _userManager.FindByEmailAsync(loginDto.UserName!);

                if (userByName == null && userByPhone == null && userByEmail == null)
                    return SignInResult.Failed;

                var user = new ApplicationUser();
                if (userByName != null)
                    user = userByName;
                else if (userByPhone != null)
                    user = userByPhone;
                else
                    user = userByEmail;

                var result = await _signInManager.PasswordSignInAsync(user!, loginDto.Password!, loginDto.RememberMe, lockoutOnFailure: false);

                return result;
            }
            catch
            {
                return new SignInResult();
            }
        }

        public async Task<string?> GetSignInUserAsync()
        {
            try
            {
                var userEntity = await _userManager.GetUserAsync(_signInManager.Context.User);
                if (userEntity != null)
                    return userEntity.Id;
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task SignInAsync(UserDto userDto, bool persistent)
        {
            try
            {
                var user = new ApplicationUser
                {
                    Id = userDto.Id!,
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    UserName = userDto.UserName,
                    Email = userDto.Email,
                    PhoneNumber = userDto.PhoneNumber,
                };

                await _signInManager.SignInAsync(user, persistent);
            }
            catch
            {
                return;
            }
        }

        public bool IsUserSignedIn()
        {
            try
            {
                return _signInManager.IsSignedIn(_signInManager.Context.User);
            }
            catch
            {
                return false;
            }
        }

        public async Task SignOutUserAsync()
        {
            try
            {
                await _signInManager.SignOutAsync();
            }
            catch
            {
                return;
            }
        }

        public async Task RefreshLoginAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    throw new Exception($"User with ID '{userId}' not found.");

                await _signInManager.RefreshSignInAsync(user);
            }
            catch
            {
                return;
            }
        }
    }
}
