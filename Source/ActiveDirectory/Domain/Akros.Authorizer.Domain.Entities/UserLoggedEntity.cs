using System.Collections.Generic;

namespace Akros.Authorizer.Domain.Entities
{
    public class UserLoggedEntity
    {
        public bool IsLogged { get; set; }
        public string? UserName { get; set; }
        public string? Name { get; set; }
        public List<Rol>? Roles { get; set; }
        public string? Message { get; set; }
        public List<string> Errors { get; set; }

    }

    public class Rol
    {
        public string? UserRol { get; set; }
    }
}
