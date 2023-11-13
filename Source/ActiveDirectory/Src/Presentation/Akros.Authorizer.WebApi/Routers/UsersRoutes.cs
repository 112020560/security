using Akros.Authorizer.Application.Features.UserFeatures.GetUsers;
using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Mvc;

namespace Akros.Authorizer.WebApi.Routers
{
    public static class UsersRoutes
    {
        public static void UseUserRoutes(this WebApplication app, ApiVersionSet apiVersionSet)
        {
            ///Version 1
            app.MapGet("api/v{version:apiVersion}/{country}", async ([FromServices] IGetUsersService getUsersService, [FromRoute] string country) => 
            { 
                return await getUsersService.ExecuteAsync(country);
            })
              .WithName("getUsers")
             .WithTags("Users")
             .WithOpenApi(op => new(op)
             {
                 Summary = "AD User",
                 Description = "Metodo encargado de autenticar los usuarios del Dominio"
             })
             .WithApiVersionSet(apiVersionSet).MapToApiVersion(new ApiVersion(1));
        }
    }
}
