using Akros.Security.UserManager.Domain.Interfaces;
using Akros.Security.UserManager.Domain.Models;
using Akros.Security.UserManager.Transversal.Common;
using MediatR;

namespace Akros.Security.UserManager.Application.Main.CQRS.Users.Queries
{
    public partial class GetUserByIdQuery: IRequest<Response<UserModel>>
    {
        public readonly string UserId;
        public GetUserByIdQuery(string UserId)
        {
            this.UserId = UserId;
        }
    }

    public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, Response<UserModel>>
    {
        private readonly IUserManagerDomain _userManagerDomain;
        public GetUserByIdHandler(IUserManagerDomain userManagerDomain)
        {
            _userManagerDomain = userManagerDomain;
        }
        public async Task<Response<UserModel>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            return new Response<UserModel>
            {
                Success = true,
                Data = await _userManagerDomain.GetUserByIdAsync(request.UserId),
            };
        }
    }
}
