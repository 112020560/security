using Akros.Security.UserManager.Application.Main.CQRS;
using Akros.Security.UserManager.Domain.Core;
using Akros.Security.UserManager.Domain.Interfaces;
using Akros.Security.UserManager.Domain.Models;
using Akros.Security.UserManager.Infrastructure.Data;
using Akros.Security.UserManager.Infrastructure.Interfaces;
using Akros.Security.UserManager.Infrastructure.Repositories;
using Asp.Versioning;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Akros.Security.UserManager.Extension;

public static class ServiceExtensions
{
    public static void AddServiceExtension(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddTransient<IUserManagerDomain, UserManagerDomain>();
        services.AddTransient<IRoleManagerDomain, RoleManagerDomain>();

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddIdentity<ApplicationUser, IdentityRole>(config => { config.SignIn.RequireConfirmedEmail = true; })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserManager<AppUserManager>()
                .AddRoles<IdentityRole>()
                .AddDefaultTokenProviders();

        services.AddTransient<IUserManagerRepository, UserManagerRepository>();
        services.AddTransient<IRolesManagerRepository, RolesManagerRepository>();

        services.AddApiVersioning(options =>
        {
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        });

    }
}
