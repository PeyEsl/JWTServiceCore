using System.ComponentModel.DataAnnotations;

namespace JWTServiceCore.Models.ViewModels
{
    public class LoginWithPhoneViewModel
    {
        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string? Phone { get; set; }

        [Display(Name = "Remember Me!")]
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
