using Akros.Security.UserManager.Domain.Interfaces;
using Akros.Security.UserManager.Domain.Models;
using Akros.Security.UserManager.Transversal.Common;
using MediatR;

namespace Akros.Security.UserManager.Application.Main.CQRS.Users.Queries
{
    public partial class GetAllUserQuery: IRequest<Response<List<UserModel>>>
    {

    }

    public class GetAllUserQueryHandler : IRequestHandler<GetAllUserQuery, Response<List<UserModel>>>
    {
        private readonly IUserManagerDomain _userManagerDomain;
        public GetAllUserQueryHandler(IUserManagerDomain userManagerDomain)
        {
            _userManagerDomain = userManagerDomain;
        }
        public async Task<Response<List<UserModel>>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
        {
            return new Response<List<UserModel>>
            {
                Success = true,
                Data = await _userManagerDomain.GetUserAsync(null)
            };
        }
    }
}
