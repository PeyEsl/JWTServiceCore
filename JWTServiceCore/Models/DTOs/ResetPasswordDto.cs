namespace JWTServiceCore.Models.DTOs
{
    public class ResetPasswordDto
    {
        public string? UserId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }
        public string? Token { get; set; }
    }
}
