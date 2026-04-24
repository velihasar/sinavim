using Business.Constants;
using Business.Handlers.Users.Commands;
using FluentValidation;

namespace Business.Handlers.Authorizations.ValidationRules
{
    public class UserChangePasswordValidator : AbstractValidator<UserChangePasswordCommand>
    {
        public UserChangePasswordValidator()
        {
            RuleFor(p => p.OldPassword)
                .NotEmpty()
                .WithMessage(Messages.PasswordEmpty);
            RuleFor(p => p.Password).Password();
        }
    }
}
