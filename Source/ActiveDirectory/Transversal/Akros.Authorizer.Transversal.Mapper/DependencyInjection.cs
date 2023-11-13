using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Akros.Authorizer.Transversal.Mapper
{
    public static class DependencyInjection
    {
        public static void AddMapperService(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
        }
    }
}
