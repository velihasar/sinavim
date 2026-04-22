using System;
using Business.Handlers.KullaniciGunlukSoruCozumus.Commands;
using Business.Handlers.KullaniciGunlukSoruCozumus.Queries;
using Core.Extensions;
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
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Tarih).NotEmpty();
            RuleFor(x => x.CozulenSoruSayisi).NotEmpty();
        }
    }

    public class UpsertMyKullaniciGunlukSoruCozumuValidator : AbstractValidator<UpsertMyKullaniciGunlukSoruCozumuCommand>
    {
        public UpsertMyKullaniciGunlukSoruCozumuValidator()
        {
            RuleFor(x => x.Tarih).NotEmpty();
            RuleFor(x => x.CozulenSoruSayisi)
                .InclusiveBetween(0, 9999)
                .WithMessage("Çözülen soru sayısı 0 ile 9.999 arasında olmalıdır.");
            RuleFor(x => x.Tarih)
                .Must(BeUtcCalendarDayWithinRetention)
                .WithMessage(
                    $"Tarih yalnızca son {GetGunlukSoruCozumuPageForMeQuery.MaxGunlukPenceresi} gün (bugün dahil, Türkiye takvimi) içinde olabilir.");
        }

        private static bool BeUtcCalendarDayWithinRetention(DateTime tarih)
        {
            var day = tarih.ToNpgsqlDateOnly();
            var today = DateTimeExtensions.TurkeyTodayToNpgsqlDateOnly();
            var oldest = today.AddDays(-(GetGunlukSoruCozumuPageForMeQuery.MaxGunlukPenceresi - 1));
            return day >= oldest && day <= today;
        }
    }
}
