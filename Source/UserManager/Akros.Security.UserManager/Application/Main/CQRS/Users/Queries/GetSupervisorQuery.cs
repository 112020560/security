using Akros.Security.UserManager.Domain.Interfaces;
using Akros.Security.UserManager.Domain.Models;
using Akros.Security.UserManager.Transversal.Common;
using MediatR;

namespace Akros.Security.UserManager.Application.Main.CQRS.Users.Queries
{
    public partial class GetSupervisorQuery: IRequest<Response<List<UserModel>>>
    {
    }

    public class GetSupervisorQueryHandler : IRequestHandler<GetSupervisorQuery, Response<List<UserModel>>>
    {
        private readonly IUserManagerDomain _userManagerDomain;
        public GetSupervisorQueryHandler(IUserManagerDomain userManagerDomain)
        {
            _userManagerDomain = userManagerDomain;
        }

        public async Task<Response<List<UserModel>>> Handle(GetSupervisorQuery request, CancellationToken cancellationToken)
        {
            var users = await _userManagerDomain.GetSupervisor();
            return new Response<List<UserModel>>
            {
                Success = true,
                Data = users
            };
        }
    }
}
