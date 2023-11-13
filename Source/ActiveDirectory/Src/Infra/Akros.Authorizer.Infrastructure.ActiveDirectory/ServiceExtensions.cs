using Akros.Authorizer.Application.Repositories;
using Akros.Authorizer.Infrastructure.ActiveDirectory.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Akros.Authorizer.Infrastructure.ActiveDirectory;

public static class ServiceExtensions
{
    public static void ConfigureActiveDirectory(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IActiveDirectoryRepository, ActiveDirectoryRepository>();
    }
}