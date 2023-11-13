using Akros.Authorizer.Application.Features.Listeners.UserCoreRegister;
using Akros.Authorizer.Application.Features.UserFeatures.Common.Interfaces;
using Akros.Authorizer.Application.Repositories;
using Akros.Authorizer.Domain.Entities.Persistence;
using Akros.Authorizer.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Akros.Authorizer.Application.Features.UserFeatures.Common;

public sealed class CoreService: ICoreService
{
    //private readonly ICoreRepository _coreRepository;
    private readonly IBrockerProducerRepository _brockerProducerRepository;
    private readonly ILogger<CoreService> _logger;
    public CoreService(IBrockerProducerRepository brockerProducerRepository, ILogger<CoreService> logger)
    {
        _brockerProducerRepository = brockerProducerRepository;
        _logger = logger;
    }


    public async Task InsertCoreRegisterAsync(UserInformationModel model, string country)
    {
        _logger.LogInformation("{username} - InsertCoreRegisterAsync", model.Username);
        CoreUser coreUser = new()
        {
            P_UPDATE = "addUserDefaultAD",
            P_USERNAME = model.Username,
            P_PASSWORD = model.Password,
            P_ROLE = "agent",
            P_STATUS = "enabled",
            P_NAME = GetStringName(model.Name, 0),
            P_LAST_NAME = string.Format("{0} {1}", GetStringName(model.Name, 1), GetStringName(model.Name, 2)),
            P_DEPARTMENT = 107,
            P_APP = 6,
            P_PAIS = 144,
            P_EXTENDED_PROPERTY = model.StrRoles
        };

        _logger.LogDebug("{username} - Informacion que se enviara al {country}_core: {data}", model.Username, country.ToUpper(), model.ToString());

        //await _coreRepository.InsertLogedRegisterAsync(coreUser, country);
        CoreMessageContract coreMessageContract = new()
        {
            Country = country,
            UserInformationModel = coreUser,
        };
        await _brockerProducerRepository.SendCoreRegisterAsync(coreMessageContract.ToString(), "akros.authenticate", "register.core");
    }

    private static string GetStringName(string? Value, int index)
    {
        if(string.IsNullOrEmpty(Value)) throw new ArgumentNullException(nameof(Value));
        var spliString = Value.Split(' ');
        var response = index >= spliString.Length ? string.Empty : spliString[index];
        return response;
    }
}
