using Akros.Authorizer.Application.Repositories;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;

namespace Akros.Authorizer.Infrastructure.Shared.Repositories;

public sealed class BrockerProducerRepository: IBrockerProducerRepository
{
    private readonly IProducingService _producingService;
    public BrockerProducerRepository(IProducingService producingService)
    {
        _producingService = producingService;
    }

    public async Task SendCoreRegisterAsync(string message, string exchange, string routingKey)
    {
        await _producingService.SendStringAsync(message, exchange, routingKey);
    }


    public async Task SendMongoLoggdRegisterAsync(string message, string exchange, string routingKey)
    {
        await _producingService.SendStringAsync(message, exchange, routingKey);
    }
}
