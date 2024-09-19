using Microsoft.AspNetCore.Identity;

namespace JWTServiceCore.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
