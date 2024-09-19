using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace JWTServiceCore.Models.ViewModels
{
    public class RegisterWithPhoneViewModel
    {
        [Key]
        public string? Id { get; set; }

        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [Remote("IsAnyPhone", "Account", HttpMethod = "Post", AdditionalFields = "__RequestVerificationToken")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Accept Policy")]
        public bool AcceptPolicy { get; set; }
    }
}