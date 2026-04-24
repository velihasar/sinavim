using Business.Handlers.Authorizations.Commands;
using FluentValidation;

namespace Business.Handlers.Authorizations.ValidationRules
{
    public class RequestEmailChangeValidator : AbstractValidator<RequestEmailChangeCommand>
    {
        public RequestEmailChangeValidator()
        {
            RuleFor(p => p.NewEmail)
                .NotEmpty()
                .WithMessage("Yeni e-posta adresi gerekli.")
                .EmailAddress()
                .WithMessage("Geçerli bir e-posta adresi girin.");
        }
    }
}
