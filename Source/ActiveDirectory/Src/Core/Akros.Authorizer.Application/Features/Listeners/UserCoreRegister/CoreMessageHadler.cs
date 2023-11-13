using Akros.Authorizer.Application.Repositories;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Core.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;
using RabbitMQ.Client.Core.DependencyInjection.Models;

namespace Akros.Authorizer.Application.Features.Listeners.UserCoreRegister;

public class CoreMessageHadler : IMessageHandler
{
    private readonly ICoreRepository _coreService;
    private readonly ILogger<CoreMessageHadler> _logger;
    public CoreMessageHadler(ICoreRepository coreService, ILogger<CoreMessageHadler> logger)
    {
        _coreService = coreService;
        _logger = logger;
    }
    public async void Handle(MessageHandlingContext context, string matchingRoute)
    {
        _logger.LogInformation($"Handling message {context.Message.GetMessage()} by routing key {matchingRoute}");
        var payload = Newtonsoft.Json.JsonConvert.DeserializeObject<CoreMessageContract>(context.Message.GetMessage()) ?? throw new Exception("No fue posible obtener la informacion del contracto");
        await _coreService.InsertLogedRegisterAsync(payload.UserInformationModel ?? throw new Exception("No se encontro un model integrado"), payload.Country ?? throw new Exception("No registro ningun pais en el contrato"));
    }
}
