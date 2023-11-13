using Akros.Authorizer.Application.Features.UserFeatures.UserAuthenticate;
using Asp.Versioning.Builder;
using Asp.Versioning.Conventions;
using Microsoft.AspNetCore.Mvc;

namespace Akros.Authorizer.WebApi.Routers
{
    public static class AuthenticateRoutes
    {
        public static void UseAuthenticateRoutes(this WebApplication app, ApiVersionSet apiVersionSet)
        {
            ///Version1
            app.MapPost("api/v{version:apiVersion}/authenticate", async (IUserAuthenticateService authService, UserAuthenticateRequest request, [FromHeader(Name = "X-KEY")] string? key) =>
            {
                return Results.Ok(await authService.ExecuteAsync(request, key));
            })
             .Produces<UserAuthenticateResponse>()
             .WithTags("Authenticate")
              .WithOpenApi(op => new(op)
              {
                  Summary = "AD User Authenticate",
                  Description = "Metodo encargado de autenticar los usuarios en el Active Directory",
                  Deprecated = true,
              })
             .WithApiVersionSet(apiVersionSet)
             .MapToApiVersion(1);

            ///Version2
            app.MapPost("api/v{version:apiVersion}/authenticate", async (IUserAuthenticateService authService, UserAuthenticateRequest request, [FromHeader(Name = "X-KEY")] string? key) =>
            {
                return Results.Ok(await authService.ExecuteV2Async(request, key));
            })
            .Produces<UserAuthenticateResponseV2>()
             .WithTags("Authenticate")
             .WithOpenApi(op => new(op)
             {
                 Summary = "AD User Authenticate",
                 Description = "Metodo encargado de autenticar los usuarios en el Active Directory"
             })
             .WithApiVersionSet(apiVersionSet)
             .MapToApiVersion(2);

            ///Version3
            app.MapPost("api/v{version:apiVersion}/authenticate", async (IUserAuthenticateService authService, UserAuthenticateRequestV3 request, [FromHeader(Name = "X-KEY")] string? key) =>
            {
                return Results.Ok(await authService.ExecuteV3Async(request, key));
            })
             .Produces<UserAuthenticateResponseV3>()
             .WithTags("Authenticate")
             .WithOpenApi(op => new(op)
             {
                 Summary = "AD User Authenticate",
                 Description = "Metodo encargado de autenticar los usuarios en el Active Directory"
             })
             .WithApiVersionSet(apiVersionSet)
             .MapToApiVersion(3);
        }
    }
}
