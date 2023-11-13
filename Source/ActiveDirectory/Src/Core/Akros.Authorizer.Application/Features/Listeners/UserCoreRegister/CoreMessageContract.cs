using Akros.Authorizer.Domain.Entities.Persistence;
using Newtonsoft.Json;

namespace Akros.Authorizer.Application.Features.Listeners.UserCoreRegister;

public sealed record CoreMessageContract
{
    public CoreUser? UserInformationModel { get; set; }
    public string? Country { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
