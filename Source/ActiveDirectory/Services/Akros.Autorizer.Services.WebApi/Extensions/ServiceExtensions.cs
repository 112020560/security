using Akros.Authorizer.Application.Main.CQRS;
using Akros.Authorizer.Domain.Core;
using Akros.Authorizer.Domain.Interfaces;
using Akros.Authorizer.Infrastructure.Directory;
using Akros.Authorizer.Infrastructure.Interface;
using Akros.Authorizer.Transversal.Mapper;
using Asp.Versioning;
using Asp.Versioning.Conventions;
using MediatR;

namespace Akros.Autorizer.Services.WebApi.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddTransversalLayer(this IServiceCollection services)
        {
            services.AddMapperService();//AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatRService();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        }

        public static void AddBusinessLayer(this IServiceCollection services)
        {
            services.AddTransient<IAuthenticateDomain, AuthenticateDomain>();
        }

        public static void AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IActiveDirectoryRepository, ActiveDirectoryRepository>();
        }

        public static void AddApiVersioningExtension(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });
        }
    }
}
