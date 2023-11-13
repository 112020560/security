using Akros.Authorizer.Application.Repositories;
using Akros.Authorizer.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Akros.Authorizer.Infrastructure.Persistence;

public static class ServiceExtensions
{
    public static void ConfigurePersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ICoreRepository, CoreRepository>();
    }
}