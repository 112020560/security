using System.ComponentModel.DataAnnotations;

namespace Akros.Authorizer.Application.Features.UserFeatures.UserAuthenticate;

public sealed record UserAuthenticateRequest
{
    [Required(ErrorMessage ="Username is required")]
    public string? User { get; set; }
    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; set; }
    public string? Cod_Pais { get; set; }
    public int? Cod_App { get; set; }
    public string? LDap { get; set; }
    public int? Encrypt { get; set; } = 0;
}
