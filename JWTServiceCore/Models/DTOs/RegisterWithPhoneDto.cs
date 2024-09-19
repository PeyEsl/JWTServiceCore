namespace JWTServiceCore.Models.DTOs
{
    public class RegisterWithPhoneDto
    {
        public string? Id { get; set; }
        public string? PhoneNumber { get; set; }
        public bool AcceptPolicy { get; set; }
    }
}
