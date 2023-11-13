using Akros.Authorizer.Application.Features.Domains.GetDomains;
using Akros.Authorizer.Application.Features.Listeners.UserCoreRegister;
using Akros.Authorizer.Application.Features.UserFeatures.Common;
using Akros.Authorizer.Application.Features.UserFeatures.Common.Interfaces;
using Akros.Authorizer.Application.Features.UserFeatures.GetUsers;
using Akros.Authorizer.Application.Features.UserFeatures.UserAuthenticate;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection;
using System.Reflection;

namespace Akros.Authorizer.Application;

public static class ServiceExtensions
{
    public static void ConfigureApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<IUserAuthenticateService, UserAuthenticateService>();
        services.AddScoped<ICoreService, CoreService>();
        services.AddScoped<IGetDomainService, GetDomainService>();
        services.AddScoped<ILoggedCacheService, LoggedCacheService>();
        services.AddScoped<IGetUsersService, GetUsersService>();

        var rabbitMqSection = configuration.GetSection("RabbitMq");
        var exchangeSection = configuration.GetSection("RabbitMqExchange");

        services.AddRabbitMqServices(rabbitMqSection)
            .AddConsumptionExchange("akros.authenticate", exchangeSection)
            .AddMessageHandlerTransient<CoreMessageHadler>("register.core")
            .AddMessageHandlerTransient<CacheLoggedHandler>("register.logged.cache");
    }
}