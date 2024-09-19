using System.ComponentModel.DataAnnotations;

namespace JWTServiceCore.Models.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Key]
        public string? Id { get; set; }

        [Display(Name = "Phone Number")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }

        [Display(Name = "Old Password")]
        [Required(ErrorMessage = "Old password is required.")]
        [DataType(DataType.Password)]
        public string? OldPassword { get; set; }

        [Display(Name = "New Password")]
        [Required(ErrorMessage = "New password is required.")]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [Display(Name = "Confirm New Password")]
        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Reset token is required.")]
        public string? Token { get; set; }
    }
}
