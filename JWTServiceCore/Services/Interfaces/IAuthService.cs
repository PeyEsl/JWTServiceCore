using JWTServiceCore.Models.DTOs;
using Microsoft.AspNetCore.Identity;

namespace JWTServiceCore.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> GenerateChangeEmailAsync(string email);
        Task<string> GenerateChangePhoneNumberAsync(string phoneNumber);
        Task<string> GenerateEmailConfirmationAsync(string userId);
        Task<IEnumerable<string>> GenerateNewTwoFactorRecoveryAsync(string userId, int number);
        Task<string> GeneratePasswordResetAsync(ResetPasswordDto resetPasswordDto);
        Task<string> GenerateTwoFactorAsync(UserDto userDto, string token);
        Task<bool> VerifyTwoFactorAsync(ConfirmDto confirmDto);
        Task<IdentityResult> EmailConfirmAsync(UserDto userDto, string token);
        Task PhoneNumberConfirmedAsync(string userId);
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<bool> CheckPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<SignInResult> LoginUserAsync(LoginDto loginDto);
        Task<string?> GetSignInUserAsync();
        Task SignInAsync(UserDto userDto, bool persistent);
        bool IsUserSignedIn();
        Task SignOutUserAsync();
        Task RefreshLoginAsync(string userId);
    }
}
