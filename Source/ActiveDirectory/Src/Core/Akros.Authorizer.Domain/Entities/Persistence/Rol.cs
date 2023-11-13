using Newtonsoft.Json;

namespace Akros.Authorizer.Domain.Entities.Persistence;

public class Rol
{
    [JsonProperty("Rol")]
    public string? UserRol { get; set; }
}
