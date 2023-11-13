using Microsoft.OpenApi.Models;

namespace Akros.Authorizer.WebApi.Extension
{
    public static class SwaggerGenExtensions
    {
        public static void ConfigureSwaggerGen(this IServiceCollection services)
        {
            var contact = new OpenApiContact()
            {
                Name = "Soporte a la Producción",
                Email = "it.soporte@multimoney.com"
            };

            var title = "Authorizer API";
            var description = "Rest API encargado de proporcionar el componente de seguridad para inicio de sesion y logeo.";

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = title,
                    Version = "v1",
                    Description = description,
                    Contact = contact
                });
                c.SwaggerDoc("v2", new OpenApiInfo
                {
                    Title = title,
                    Version = "v2",
                    Description = description,
                    Contact = contact
                });
                c.SwaggerDoc("v3", new OpenApiInfo
                {
                    Title = title,
                    Version = "v3",
                    Description = description,
                    Contact = contact
                });
            });

        }
    }
}
