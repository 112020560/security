using Akros.Authorizer.Application.Repositories;
using AutoMapper;

namespace Akros.Authorizer.Application.Features.UserFeatures.GetUsers;

public sealed class GetUsersService: IGetUsersService
{
    private readonly IActiveDirectoryRepository _activeDirectoryRepository;
    private readonly ICountryRepository _countryRepository;
    private readonly IMapper _mapper;
    public GetUsersService(IActiveDirectoryRepository activeDirectoryRepository, ICountryRepository countryRepository, IMapper mapper)
    {
        _activeDirectoryRepository = activeDirectoryRepository;
        _countryRepository = countryRepository;
        _mapper = mapper;
    }

    public async Task<List<GetUserResponse>> ExecuteAsync(string country)
    {
        var country_ldpa = await _countryRepository.GetLdpaByCountryAsync(country) ?? throw new Exception("No se encontro la información de los ldaps");
        var entityList = await _activeDirectoryRepository.GetADUsers(country_ldpa.Split("|"));
        return _mapper.Map<List<GetUserResponse>>(entityList);
    }
}
