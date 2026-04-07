
using Business.Handlers.KullaniciKonuIlerlemes.Commands;
using FluentValidation;

namespace Business.Handlers.KullaniciKonuIlerlemes.ValidationRules
{

    public class CreateKullaniciKonuIlerlemeValidator : AbstractValidator<CreateKullaniciKonuIlerlemeCommand>
    {
        public CreateKullaniciKonuIlerlemeValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.KonuId).NotEmpty();
            RuleFor(x => x.Durum).NotEmpty();

        }
    }
    public class UpdateKullaniciKonuIlerlemeValidator : AbstractValidator<UpdateKullaniciKonuIlerlemeCommand>
    {
        public UpdateKullaniciKonuIlerlemeValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.KonuId).NotEmpty();
            RuleFor(x => x.Durum).NotEmpty();

        }
    }
}