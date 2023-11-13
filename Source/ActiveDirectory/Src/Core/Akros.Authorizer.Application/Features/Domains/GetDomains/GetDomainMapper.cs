using Akros.Authorizer.Domain.Entities.Persistence;
using AutoMapper;

namespace Akros.Authorizer.Application.Features.Domains.GetDomains;

public class GetDomainMapper: Profile
{
    public GetDomainMapper()
    {
        CreateMap<DomainModel, GetDomainResponse>();
    }
}
