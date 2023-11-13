using Akros.Security.UserManager.Application.Dtos;
using Akros.Security.UserManager.Application.Main.CQRS.Users.Command;
using Akros.Security.UserManager.Application.Main.CQRS.Users.Queries;
using Asp.Versioning;
using Asp.Versioning.Builder;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Akros.Security.UserManager.Routes
{
    public static class UserManagerRoutes
    {
        
        //Metodo encargado de crear el usuario
        public static void UseUserRoutes(this WebApplication app, ApiVersionSet apiVersionSet)
        {
            app.MapGet("api/v{version:apiVersion}/user", async (IMediator mediator) =>
            {
                return await mediator.Send(new GetAllUserQuery());
            })
                .WithName("GetUsers")
                .WithApiVersionSet(apiVersionSet).MapToApiVersion(new ApiVersion(1));

            app.MapGet("api/v{version:apiVersion}/user/{findparam}", async (IMediator mediator, [FromRoute] string findparam) =>
            {
                return await mediator.Send(new GetUserByParamQuery(findparam));
            })
                .WithName("GetUserByParam")
                .WithApiVersionSet(apiVersionSet).MapToApiVersion(new ApiVersion(1));

            app.MapGet("api/v{version:apiVersion}/user/byId/{id}", async (IMediator mediator, [FromRoute] string id) =>
            {
                return await mediator.Send(new GetUserByIdQuery(id));
            })
                .WithName("GetUserById")
                .WithApiVersionSet(apiVersionSet).MapToApiVersion(new ApiVersion(1));

            app.MapGet("api/v{version:apiVersion}/supervisor", async (IMediator mediator) =>
            {
                return await mediator.Send(new GetSupervisorQuery());
            })
                .WithName("GetSupervisors")
                .WithApiVersionSet(apiVersionSet).MapToApiVersion(new ApiVersion(1));

            app.MapPost("api/v{version:apiVersion}/user", async (IMediator mediator, CreateUserDto createUserDto) =>
            {
                return Results.Created("", await mediator.Send(new CreateUserCommand(createUserDto)));
            })
                .WithName("CreateUser")
                .WithApiVersionSet(apiVersionSet).MapToApiVersion(new ApiVersion(1));


            app.MapPost("api/v{version:apiVersion}/users", async (IMediator mediator, List<CreateUserDto> createUserDtoList) =>
            {
                return Results.Created("", await mediator.Send(new CreateManyUserCommand(createUserDtoList)));
            })
                .WithName("CreateUsers")
                .WithApiVersionSet(apiVersionSet).MapToApiVersion(new ApiVersion(1));


            app.MapPut("api/v{version:apiVersion}/user", async (IMediator mediator, CreateUserDto createUserDto) =>
            {
                return Results.Created("", await mediator.Send(new UpdateUserCommand(createUserDto)));
            })
                .WithName("UpdateUser")
                .WithApiVersionSet(apiVersionSet).MapToApiVersion(new ApiVersion(1));


            app.MapPut("api/v{version:apiVersion}/changepassword", async (IMediator mediator, ChangePasswordDto createUserDto) =>
            {
                return Results.Created("", await mediator.Send(new ChangePasswordCommand(createUserDto)));
            })
                .WithName("ChangePassword")
                .WithApiVersionSet(apiVersionSet).MapToApiVersion(new ApiVersion(1));
        }
    }
}
