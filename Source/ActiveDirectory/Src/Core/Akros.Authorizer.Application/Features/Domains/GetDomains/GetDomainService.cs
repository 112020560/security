using Akros.Authorizer.Application.Repositories;
using AutoMapper;

namespace Akros.Authorizer.Application.Features.Domains.GetDomains;

public sealed class GetDomainService: IGetDomainService
{
    private readonly ICountryRepository _countryRepository;
    private readonly IMapper _mapper;
    public GetDomainService(ICountryRepository countryRepository, IMapper mapper)
    {
        _countryRepository = countryRepository;
        _mapper = mapper;
    }

    public async Task<List<GetDomainResponse>> ExecuteAsync()
    {
        var model = await _countryRepository.GetDomainByContry();
        return _mapper.Map<List<GetDomainResponse>>(model);
    }
}
