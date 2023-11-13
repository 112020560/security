using Akros.Security.UserManager.Domain.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Akros.Security.UserManager.Infrastructure.Interfaces
{
    public interface IUserManagerRepository
    {
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
        Task<ApplicationUser> GetUserByUserNameAsync(string username);
        Task<bool> CreateUserRoleAsync(ApplicationUser user, string[] roles);
        Task<bool> CreateUserListClaimAsync(ApplicationUser user, Dictionary<string, string> claims);
        Task RemoveUserClaimAsync(ApplicationUser user, Claim claim);
        Task<IdentityResult> UpdateUserAsync(ApplicationUser user);
        Task<List<Claim>> GetUserCliamsAsync(ApplicationUser user);
        Task<bool> CreateUserClaimAsync(ApplicationUser user, Claim claim);
        Task ChangePasswordAsync(ApplicationUser user, string curretPassword, string newPassword);
        Task<List<ApplicationUser>> GetUserAsync();
        Task<List<ApplicationUser>> GetUserAsync(Expression<Func<ApplicationUser, bool>> filterExpression);
        Task<IList<string>> GetUserRoleAsync(ApplicationUser user);
        Task<ApplicationUser> GetUserByIdAsync(string id);
        Task<bool> DeleteUserRolesAsync(ApplicationUser user, string[] roles);
    }
}
