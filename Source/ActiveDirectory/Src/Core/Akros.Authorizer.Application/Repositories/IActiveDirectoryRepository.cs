using Akros.Authorizer.Domain.Entities.Persistence;
using Akros.Authorizer.Domain.Models;

namespace Akros.Authorizer.Application.Repositories
{
    public interface IActiveDirectoryRepository
    {
        Task<(bool authenticate, UserInformationModel userInformation)> ActiveDirectoryAuthenticateAsync(User userEntity);
        Task<List<User>> GetADUsers(string[] ldaps);
    }
}
