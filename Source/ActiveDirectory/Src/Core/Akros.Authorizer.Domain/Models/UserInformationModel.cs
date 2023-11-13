using Newtonsoft.Json;

namespace Akros.Authorizer.Domain.Models;

public sealed record UserInformationModel
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Name { get; set; }
    public List<string>? Roles { get; set; }
    public string? StrRoles { get; set; }
    public string? Email { get; set; }
    public string? LastLogon { get; set; }
    public string? Country { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
