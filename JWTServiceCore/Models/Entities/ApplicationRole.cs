using Microsoft.AspNetCore.Identity;

namespace JWTServiceCore.Models.Entities
{
    public class ApplicationRole : IdentityRole
    {
        public string? Description { get; set; }
    }
}
