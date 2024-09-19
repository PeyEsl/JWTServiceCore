using System.ComponentModel.DataAnnotations;

namespace JWTServiceCore.Models.ViewModels
{
    public class UpdateProfileViewModel
    {
        [Key]
        public string? Id { get; set; }

        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Display(Name = "User Name")]
        [Required(ErrorMessage = "User name is required.")]
        public string? UserName { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }

        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Roles")]
        public IEnumerable<string>? Roles { get; set; }
        public IEnumerable<string>? RoleList { get; set; }

        [Display(Name = "Claims")]
        public IList<ClaimViewModel>? Claims { get; set; }
    }
}
