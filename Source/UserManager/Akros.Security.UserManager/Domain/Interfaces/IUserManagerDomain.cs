using Akros.Security.UserManager.Application.Dtos;
using Akros.Security.UserManager.Domain.Models;

namespace Akros.Security.UserManager.Domain.Interfaces
{
    public interface IUserManagerDomain
    {
        Task<bool> CreateUserAsync(CreateUserDto createUserDto);
        Task UpdateUserAsync(CreateUserDto createUserDto);
        Task ChangePasswordAsync(ChangePasswordDto changePasswordDto);
        Task<List<UserModel>> GetUserAsync(string? activeParam);
        Task<UserModel> GetUserByIdAsync(string? id);
        Task<List<UserModel>> GetSupervisor();
    }
}
