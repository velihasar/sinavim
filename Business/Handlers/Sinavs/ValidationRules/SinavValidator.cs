
using Business.Handlers.Sinavs.Commands;
using FluentValidation;

namespace Business.Handlers.Sinavs.ValidationRules
{

    public class CreateSinavValidator : AbstractValidator<CreateSinavCommand>
    {
        public CreateSinavValidator()
        {
            RuleFor(x => x.Ad).NotEmpty();
            RuleFor(x => x.Tarih)
                .NotEmpty()
                .When(x => x.TahminiTarih == null)
                .WithMessage("Tarih veya Tahmini Tarih alanlarından en az biri dolu olmalıdır.");
            RuleFor(x => x.TahminiTarih)
                .NotEmpty()
                .When(x => x.Tarih == null)
                .WithMessage("Tarih veya Tahmini Tarih alanlarından en az biri dolu olmalıdır.");
            RuleFor(x => x.SiraNo).GreaterThanOrEqualTo(0);
        }
    }
    public class UpdateSinavValidator : AbstractValidator<UpdateSinavCommand>
    {
        public UpdateSinavValidator()
        {
            RuleFor(x => x.Ad).NotEmpty();
            RuleFor(x => x.Tarih)
                .NotEmpty()
                .When(x => x.TahminiTarih == null)
                .WithMessage("Tarih veya Tahmini Tarih alanlarından en az biri dolu olmalıdır.");
            RuleFor(x => x.TahminiTarih)
                .NotEmpty()
                .When(x => x.Tarih == null)
                .WithMessage("Tarih veya Tahmini Tarih alanlarından en az biri dolu olmalıdır.");
            RuleFor(x => x.SiraNo).GreaterThanOrEqualTo(0);
        }
    }
}