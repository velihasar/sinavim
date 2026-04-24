using Business.Handlers.Authorizations.Commands;
using FluentValidation;

namespace Business.Handlers.Authorizations.ValidationRules
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserValidator()
        {
            RuleFor(p => p.Email)
                .NotEmpty()
                .WithMessage("E-posta adresi gerekli.")
                .EmailAddress()
                .WithMessage("Geçerli bir e-posta adresi girin.");
            RuleFor(p => p.FullName)
                .NotEmpty()
                .WithMessage("Ad soyad gerekli.")
                .MaximumLength(200)
                .WithMessage("Ad soyad en fazla 200 karakter olabilir.");
            RuleFor(p => p.Password).Password();
        }
    }
}