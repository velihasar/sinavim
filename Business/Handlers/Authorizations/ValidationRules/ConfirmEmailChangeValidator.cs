using Business.Handlers.Authorizations.Commands;
using FluentValidation;

namespace Business.Handlers.Authorizations.ValidationRules
{
    public class ConfirmEmailChangeValidator : AbstractValidator<ConfirmEmailChangeCommand>
    {
        public ConfirmEmailChangeValidator()
        {
            RuleFor(p => p.Code)
                .NotEmpty()
                .WithMessage("Doğrulama kodu gerekli.");
        }
    }
}
