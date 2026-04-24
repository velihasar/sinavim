using Business.Handlers.Authorizations.Commands;
using FluentValidation;

namespace Business.Handlers.Authorizations.ValidationRules
{
    public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailCommand>
    {
        public ConfirmEmailValidator()
        {
            RuleFor(p => p.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(50);
            RuleFor(p => p.Code)
                .NotEmpty()
                .Length(6)
                .Matches(@"^\d{6}$").WithMessage("Kod 6 haneli olmalıdır.");
        }
    }
}
