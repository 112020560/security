using Akros.Authorizer.Application.Dto;
using Akros.Authorizer.Domain.Entities;
using Akros.Authorizer.Domain.Interfaces;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Akros.Authorizer.Application.Main.CQRS.Users.Queries
{
    public partial class UserAuthenticateQuery: IRequest<UserLoggedEntity>
    {
        public readonly UserLoggedDto userLoggedDto;
        public UserAuthenticateQuery(UserLoggedDto userLoggedDto)
        {
            this.userLoggedDto = userLoggedDto;
        }
    }

    public class UserAuthenticateHandler : IRequestHandler<UserAuthenticateQuery, UserLoggedEntity>
    {
        private readonly IMapper _mapper;
        private readonly IAuthenticateDomain _authenticateDomain;
        public UserAuthenticateHandler(IMapper mapper, IAuthenticateDomain authenticateDomain)
        {
            _mapper = mapper;
            _authenticateDomain = authenticateDomain;
        }
        public async Task<UserLoggedEntity> Handle(UserAuthenticateQuery request, CancellationToken cancellationToken)
        {
            var model = _mapper.Map<UserModel>(request.userLoggedDto);
            return await _authenticateDomain.UserAuthenticateAsync(model, cancellationToken);
        }
    }
}
