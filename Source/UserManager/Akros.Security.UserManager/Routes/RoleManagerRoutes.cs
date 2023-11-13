using Akros.Security.UserManager.Application.Main.CQRS.Roles.Commands;
using Akros.Security.UserManager.Application.Main.CQRS.Roles.Queries;
using Akros.Security.UserManager.Domain.Models;
using Asp.Versioning;
using Asp.Versioning.Builder;
using MediatR;

namespace Akros.Security.UserManager.Routes
{
    public static class RoleManagerRoutes
    {
        public static void UseRoleRoutes(this WebApplication app, ApiVersionSet apiVersionSet)
        {
            app.MapPost("api/v{version:apiVersion}/role", async (IMediator mediator, RoleModel createUserDto) =>
            {
                return Results.Created("", await mediator.Send(new CreteRoleCommand(createUserDto)));
            })
                .WithName("CreateRole")
                .WithApiVersionSet(apiVersionSet).MapToApiVersion(new ApiVersion(1));


            app.MapGet("api/v{version:apiVersion}/role", async (IMediator mediator) =>
            {
                return await mediator.Send(new RolesGetAllQuery());
            })
                .WithName("GetAllRoles")
                .WithApiVersionSet(apiVersionSet).MapToApiVersion(new ApiVersion(1));

            app.MapPut("api/v{version:apiVersion}/role", async (IMediator mediator, RoleModel createUserDto) =>
            {
                return Results.Created("UpdateRole", await mediator.Send(new UpdateRoleCommand(createUserDto)));
            })
                .WithName("UpdateRole")
                .WithApiVersionSet(apiVersionSet).MapToApiVersion(new ApiVersion(1));
        }
    }
}
