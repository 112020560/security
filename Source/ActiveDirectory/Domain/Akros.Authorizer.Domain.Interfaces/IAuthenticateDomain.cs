using Akros.Authorizer.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Akros.Authorizer.Domain.Interfaces
{
    public interface IAuthenticateDomain
    {
        Task<UserLoggedEntity> UserAuthenticateAsync(UserModel userModel, CancellationToken cancellationToken = default);
    }
}
