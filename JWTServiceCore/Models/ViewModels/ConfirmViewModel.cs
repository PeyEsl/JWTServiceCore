using System.ComponentModel.DataAnnotations;

namespace JWTServiceCore.Models.ViewModels
{
    public class ConfirmViewModel
    {
        [Key]
        public string? Id { get; set; }

        [Display(Name = "Phone Number")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }
        public string? Token { get; set; }

        [Required(ErrorMessage = "SMS Code is required.")]
        [StringLength(6)]
        public string? Code { get; set; }

        [Display(Name = "Received Code")]
        [StringLength(6)]
        public string? SendCode { get; set; }
        public bool ResetPassword { get; set; }
        public string? CallBackUrl { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
