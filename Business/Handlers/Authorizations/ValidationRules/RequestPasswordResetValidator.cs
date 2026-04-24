using Business.Handlers.Authorizations.Commands;
using FluentValidation;

namespace Business.Handlers.Authorizations.ValidationRules
{
    public class RequestPasswordResetValidator : AbstractValidator<RequestPasswordResetCommand>
    {
        public RequestPasswordResetValidator()
        {
            RuleFor(p => p.Email)
                .NotEmpty()
                .WithMessage("E-posta adresi gerekli.")
                .EmailAddress()
                .WithMessage("Geçerli bir e-posta adresi girin.");
        }
    }
}
