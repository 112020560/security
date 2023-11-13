namespace Akros.Authorizer.Application.Features.Domains.GetDomains;

public interface IGetDomainService
{
    Task<List<GetDomainResponse>> ExecuteAsync();
}
