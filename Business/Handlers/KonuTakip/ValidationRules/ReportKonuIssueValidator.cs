using Business.Handlers.KonuTakip.Commands;
using FluentValidation;

namespace Business.Handlers.KonuTakip.ValidationRules
{
    public class ReportKonuIssueValidator : AbstractValidator<ReportKonuIssueCommand>
    {
        public ReportKonuIssueValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage("Ad soyad gereklidir.")
                .MaximumLength(200)
                .WithMessage("Ad soyad en fazla 200 karakter olabilir.");

            RuleFor(x => x.UserEmail)
                .NotEmpty()
                .WithMessage("E-posta gereklidir.")
                .EmailAddress()
                .WithMessage("Geçerli bir e-posta adresi girin.");

            RuleFor(x => x.Subject)
                .NotEmpty()
                .WithMessage("Bildirim türü gereklidir.")
                .MaximumLength(200)
                .WithMessage("Bildirim türü en fazla 200 karakter olabilir.");

            RuleFor(x => x.Message)
                .NotEmpty()
                .WithMessage("Açıklama gereklidir.")
                .MaximumLength(8000)
                .WithMessage("Açıklama çok uzun.");

            RuleFor(x => x.SinavAd)
                .MaximumLength(300)
                .WithMessage("Sınav adı çok uzun.");

            RuleFor(x => x.SinavBolumAd)
                .MaximumLength(300)
                .WithMessage("Bölüm adı çok uzun.");
        }
    }
}
