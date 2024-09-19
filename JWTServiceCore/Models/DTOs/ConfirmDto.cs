namespace JWTServiceCore.Models.DTOs
{
    public class ConfirmDto
    {
        public string? UserId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
        public string? Code { get; set; }
        public string? TokenProvider { get; set; }
        public bool ResetPassword { get; set; }
    }
}
