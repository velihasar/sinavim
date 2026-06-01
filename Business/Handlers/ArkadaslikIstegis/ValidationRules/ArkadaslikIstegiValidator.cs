
using Business.Handlers.ArkadaslikIstegis.Commands;
using FluentValidation;

namespace Business.Handlers.ArkadaslikIstegis.ValidationRules
{

    public class CreateArkadaslikIstegiValidator : AbstractValidator<CreateArkadaslikIstegiCommand>
    {
        public CreateArkadaslikIstegiValidator()
        {
            RuleFor(x => x.GonderenUserId).GreaterThan(0);
            RuleFor(x => x.HedefUserId).GreaterThan(0);
            RuleFor(x => x.KullanilanDavetKodu).NotEmpty().MaximumLength(16);
        }
    }
    public class UpdateArkadaslikIstegiValidator : AbstractValidator<UpdateArkadaslikIstegiCommand>
    {
        public UpdateArkadaslikIstegiValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.GonderenUserId).GreaterThan(0);
            RuleFor(x => x.HedefUserId).GreaterThan(0);
            RuleFor(x => x.KullanilanDavetKodu).MaximumLength(16);
        }
    }
}