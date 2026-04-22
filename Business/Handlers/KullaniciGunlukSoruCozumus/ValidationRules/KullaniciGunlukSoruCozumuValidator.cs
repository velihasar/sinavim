
using Business.Handlers.KullaniciGunlukSoruCozumus.Commands;
using FluentValidation;

namespace Business.Handlers.KullaniciGunlukSoruCozumus.ValidationRules
{

    public class CreateKullaniciGunlukSoruCozumuValidator : AbstractValidator<CreateKullaniciGunlukSoruCozumuCommand>
    {
        public CreateKullaniciGunlukSoruCozumuValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Tarih).NotEmpty();
            RuleFor(x => x.CozulenSoruSayisi).NotEmpty();

        }
    }
    public class UpdateKullaniciGunlukSoruCozumuValidator : AbstractValidator<UpdateKullaniciGunlukSoruCozumuCommand>
    {
        public UpdateKullaniciGunlukSoruCozumuValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Tarih).NotEmpty();
            RuleFor(x => x.CozulenSoruSayisi).NotEmpty();

        }
    }
}