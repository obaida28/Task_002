using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Core.Entites;
public class ApplicationUser : IdentityUser
{
    [MaxLength(50)]
    public string FirstName { get; set; }

    [MaxLength(50)]
    public string LastName { get; set; }

    public List<RefreshToken>? RefreshTokens { get; set; }
}