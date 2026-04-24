using Business.Handlers.Authorizations.Commands;
using FluentValidation;

namespace Business.Handlers.Authorizations.ValidationRules
{
    public class ConfirmPasswordResetValidator : AbstractValidator<ConfirmPasswordResetCommand>
    {
        public ConfirmPasswordResetValidator()
        {
            RuleFor(p => p.Email)
                .NotEmpty()
                .WithMessage("E-posta adresi gerekli.")
                .EmailAddress()
                .WithMessage("Geçerli bir e-posta adresi girin.");
            RuleFor(p => p.Code)
                .NotEmpty()
                .WithMessage("Sıfırlama kodu gerekli.")
                .Length(6)
                .WithMessage("Kod 6 haneli olmalıdır.")
                .Matches("^[0-9]+$")
                .WithMessage("Kod yalnızca rakamlardan oluşmalıdır.");
            RuleFor(p => p.NewPassword).Password();
        }
    }
}
