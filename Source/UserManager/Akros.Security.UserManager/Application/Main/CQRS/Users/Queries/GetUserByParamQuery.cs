using Akros.Security.UserManager.Domain.Interfaces;
using Akros.Security.UserManager.Domain.Models;
using Akros.Security.UserManager.Transversal.Common;
using MediatR;

namespace Akros.Security.UserManager.Application.Main.CQRS.Users.Queries
{
    public partial class GetUserByParamQuery : IRequest<Response<List<UserModel>>>
    {
        public readonly string searchParam;
        public GetUserByParamQuery(string searchParam)
        {
            this.searchParam = searchParam;
        }
    }

    public class GetUserByParamQueryHandler : IRequestHandler<GetUserByParamQuery, Response<List<UserModel>>>
    {
        private readonly IUserManagerDomain _userManagerDomain;
        public GetUserByParamQueryHandler(IUserManagerDomain userManagerDomain)
        {
            _userManagerDomain = userManagerDomain;
        }

        public async Task<Response<List<UserModel>>> Handle(GetUserByParamQuery request, CancellationToken cancellationToken)
        {
            return new Response<List<UserModel>>
            {
                Success = true,
                Data = await _userManagerDomain.GetUserAsync(request.searchParam)
            };
        }
    }
}
