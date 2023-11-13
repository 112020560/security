using Akros.Authorizer.Application.Repositories;
using Akros.Authorizer.Infrastructure.Shared.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection;

namespace Akros.Authorizer.Infrastructure.Shared
{
    public static class ServiceExtensions
    {
        public static void ConfigureShared(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IEncryptRepository, EncryptRepository>();
            services.AddScoped<IContextTraceRepository, ContextTraceRepository>();

            var rabbitMqSection = configuration.GetSection("RabbitMq");
            var exchangeSection = configuration.GetSection("RabbitMqExchange");

            services.AddRabbitMqProducer(rabbitMqSection)
                .AddProductionExchange("akros.authenticate", exchangeSection);

            services.AddScoped<IBrockerProducerRepository, BrockerProducerRepository>();
        }

    }
}