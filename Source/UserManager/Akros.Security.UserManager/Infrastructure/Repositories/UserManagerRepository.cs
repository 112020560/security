using Akros.Security.UserManager.Domain.Models;
using Akros.Security.UserManager.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Akros.Security.UserManager.Infrastructure.Repositories
{
    public class UserManagerRepository: IUserManagerRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserManagerRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<ApplicationUser>> GetUserAsync(Expression<Func<ApplicationUser, bool>> filterExpression)
        {
            return await _userManager.Users.Where(filterExpression).ToListAsync();
        }

        public async Task<List<ApplicationUser>> GetUserAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<List<Claim>> GetUserCliamsAsync(ApplicationUser user)
        {
            return (await _userManager.GetClaimsAsync(user)).ToList();
        }

        public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<ApplicationUser> GetUserByUserNameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<bool> CreateUserRoleAsync(ApplicationUser user, string[] roles)
        {
            await _userManager.AddToRolesAsync(user, roles);
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteUserRolesAsync(ApplicationUser user, string[] roles)
        {
            await _userManager.RemoveFromRolesAsync(user, roles);
            return true;
        }

        public async Task<IList<string>> GetUserRoleAsync(ApplicationUser user)
        {

            return await _userManager.GetRolesAsync(user);
        }

        public async Task<bool> CreateUserListClaimAsync(ApplicationUser user, Dictionary<string, string> claims)
        {
            foreach (var claim in claims)
            {
                await _userManager.AddClaimAsync(user, new Claim(claim.Key, claim.Value));
            }
            return await Task.FromResult(true);
        }

        public async Task<bool> CreateUserClaimAsync(ApplicationUser user, Claim claim)
        {
            await _userManager.AddClaimAsync(user, claim);
            return await Task.FromResult(true);
        }

        public async Task ChangePasswordAsync(ApplicationUser user, string curretPassword, string newPassword)
        { 
            await _userManager.ChangePasswordAsync(user, curretPassword, newPassword);
        }

        public async Task RemoveUserClaimAsync(ApplicationUser user, Claim claim)
        {
            await _userManager.RemoveClaimAsync(user, claim);
        }
    }
}
