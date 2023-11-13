using Akros.Authorizer.Domain.Entities;
using System.Threading.Tasks;

namespace Akros.Authorizer.Infrastructure.Interface
{
    public interface IActiveDirectoryRepository
    {
        Task<UserLoggedEntity> Authenticate(UserModel userModel);
        Task<bool> CreateUserAsync(CreateAdUserModel createAdUserModel);
    }
}
