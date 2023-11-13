using Akros.Authorizer.Application.Repositories;
using Akros.Authorizer.Domain.Settings;
using Akros.Authorizer.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Akros.Authorizer.Infrastructure.Mongo;

public static class ServiceExtensions
{
    public static void ConfigureMongoPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));

        services.AddSingleton<IMongoDbSettings>(serviceProvider =>
            serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value);

        services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));
        services.AddScoped<ICountryRepository, CountryRepository>();
        services.AddTransient<ILoggedCacheRepository, LoggedCacheRepository>();
    }
}