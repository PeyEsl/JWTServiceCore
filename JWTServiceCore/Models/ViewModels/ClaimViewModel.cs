using System.ComponentModel.DataAnnotations;

namespace JWTServiceCore.Models.ViewModels
{
    public class ClaimViewModel
    {
        [Key]
        public string? Id { get; set; }

        [Display(Name = "Role Name")]
        public string? RoleName { get; set; }
        public string? UserId { get; set; }

        [Display(Name = "User Name")]
        public string? UserName { get; set; }
        public string? ClaimType { get; set; }

        [Display(Name = "Claim Type")]
        [Required(ErrorMessage = "Claim type is required.")]
        public string? Type { get; set; }

        [Display(Name = "Claim Value")]
        [Required(ErrorMessage = "Claim value is required.")]
        public string? Value { get; set; }
    }
}
