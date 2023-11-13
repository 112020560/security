using Microsoft.AspNetCore.Identity;

namespace Akros.Security.UserManager.Domain.Models;

public class ApplicationUser : IdentityUser
{
    public bool IsActive { get; set; }
    public string Country { get; set; }
    public bool ActiveDirectory { get; set; }
}
