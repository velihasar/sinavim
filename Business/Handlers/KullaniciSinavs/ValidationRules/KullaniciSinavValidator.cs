
using Business.Handlers.KullaniciSinavs.Commands;
using FluentValidation;

namespace Business.Handlers.KullaniciSinavs.ValidationRules
{

    public class CreateKullaniciSinavValidator : AbstractValidator<CreateKullaniciSinavCommand>
    {
        public CreateKullaniciSinavValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.SinavId).NotEmpty();
            RuleFor(x => x.HedefPuan).NotEmpty();

        }
    }
    public class UpdateKullaniciSinavValidator : AbstractValidator<UpdateKullaniciSinavCommand>
    {
        public UpdateKullaniciSinavValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.SinavId).NotEmpty();
            RuleFor(x => x.HedefPuan).NotEmpty();

        }
    }
}