using System.ComponentModel.DataAnnotations;

namespace JWTServiceCore.Models.ViewModels
{
    public class LoginViewModel
    {
        [Key]
        public string? Id { get; set; }

        [Display(Name = "User Name")]
        [Required(ErrorMessage = "Phone number is required.")]
        public string? UserName { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string? Password { get; set; }

        [Display(Name = "Remember Me!")]
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
