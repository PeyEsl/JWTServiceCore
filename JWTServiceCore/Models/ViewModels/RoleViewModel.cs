using System.ComponentModel.DataAnnotations;

namespace JWTServiceCore.Models.ViewModels
{
    public class RoleViewModel
    {
        [Key]
        public string? Id { get; set; }

        [Display(Name = "User Name")]
        public string? UserName { get; set; }

        [Display(Name = "Name Role")]
        [Required(ErrorMessage = "Role name is required.")]
        public string? Name { get; set; }

        [Display(Name = "Claims")]
        public IList<ClaimViewModel>? Claims { get; set; }
    }
}
