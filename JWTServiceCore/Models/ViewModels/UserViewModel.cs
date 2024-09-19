using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace JWTServiceCore.Models.ViewModels
{
    public class UserViewModel
    {
        [Key]
        public string? Id { get; set; }

        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Display(Name = "User Name")]
        [Required(ErrorMessage = "User name is required.")]
        [Remote("IsAnyUserName", "Account", HttpMethod = "Post", AdditionalFields = "__RequestVerificationToken")]
        public string? UserName { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Remote("IsAnyEmail", "Account", HttpMethod = "Post", AdditionalFields = "__RequestVerificationToken")]
        public string? Email { get; set; }

        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [Remote("IsAnyPhone", "Account", HttpMethod = "Post", AdditionalFields = "__RequestVerificationToken")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string? Password { get; set; }

        [Display(Name = "Confirm Password")]
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }

        [Display(Name = "Accept Policy")]
        public bool AcceptPolicy { get; set; }

        [Display(Name = "Roles")]
        public IEnumerable<string>? Roles { get; set; }

        [Display(Name = "Claims")]
        public IEnumerable<ClaimViewModel>? Claims { get; set; }
    }
}
