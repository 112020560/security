using Akros.Authorizer.Application.Repositories;
using Akros.Authorizer.Domain.Entities.Mongo;
using AutoMapper;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Core.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;
using RabbitMQ.Client.Core.DependencyInjection.Models;

namespace Akros.Authorizer.Application.Features.Listeners.UserCoreRegister;

public sealed class CacheLoggedHandler : IMessageHandler
{
    private readonly ILoggedCacheRepository _loggedCacheRepository;
    private readonly ILogger<CacheLoggedHandler> _logger;
    private readonly IMapper _mapper;
    public CacheLoggedHandler(ILoggedCacheRepository loggedCacheRepository, ILogger<CacheLoggedHandler> logger, IMapper mapper)
    {
        _loggedCacheRepository = loggedCacheRepository;
        _logger = logger;
        _mapper = mapper;
    }
    public async void Handle(MessageHandlingContext context, string matchingRoute)
    {
        _logger.LogInformation($"Handling message {context.Message.GetMessage()} by routing key {matchingRoute}");
        var contract = Newtonsoft.Json.JsonConvert.DeserializeObject<CacheLoggedContract>(context.Message.GetMessage()) ?? throw new Exception("No fue posible obtener la informacion del contracto");
        var entity = _mapper.Map<LoggedCache>(contract);
        await _loggedCacheRepository.CreateLoggedCacheAsync(entity);
    }
}
