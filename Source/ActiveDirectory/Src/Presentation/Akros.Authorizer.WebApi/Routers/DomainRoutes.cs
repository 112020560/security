using Akros.Authorizer.Application.Features.Domains.GetDomains;
using Asp.Versioning;
using Asp.Versioning.Builder;

namespace Akros.Authorizer.WebApi.Routers
{
    public static class DomainRoutes
    {
        public static void UseDomainRoutes(this WebApplication app, ApiVersionSet apiVersionSet)
        {
            ///version1
            app.MapGet("api/v{version:apiVersion}/domains", async (IGetDomainService _getDomainService) =>
            {
                return await _getDomainService.ExecuteAsync();
            })
             .WithName("getDomains")
             .WithTags("Domains")
             .WithOpenApi(op => new(op)
             {
                 Summary = "AD Domains",
                 Description = "Metodo encargado de obtener el listado de dominios"
             })
             .WithApiVersionSet(apiVersionSet).MapToApiVersion(new ApiVersion(1)); ;
        }
    }
}
