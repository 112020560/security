namespace Akros.Authorizer.Application.Repositories
{
    public interface IBrockerProducerRepository
    {
        Task SendCoreRegisterAsync(string message, string exchange, string routingKey);
        Task SendMongoLoggdRegisterAsync(string message, string exchange, string routingKey);
    }
}
