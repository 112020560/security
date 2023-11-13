using FluentValidation;

namespace Akros.Authorizer.Application.Features.UserFeatures.UserAuthenticate
{
    public sealed class UserAuthenticateValidator : AbstractValidator<UserAuthenticateRequest>
    {
        public UserAuthenticateValidator()
        {
            RuleFor(x => x.User).NotEmpty().WithMessage("User property is required");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password property is required");
        }
    }
}
