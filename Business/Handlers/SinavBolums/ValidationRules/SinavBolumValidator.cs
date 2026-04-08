
using Business.Handlers.SinavBolums.Commands;
using FluentValidation;

namespace Business.Handlers.SinavBolums.ValidationRules
{

    public class CreateSinavBolumValidator : AbstractValidator<CreateSinavBolumCommand>
    {
        public CreateSinavBolumValidator()
        {
            RuleFor(x => x.Isim).NotEmpty();
            RuleFor(x => x.SinavId).GreaterThan(0);
        }
    }

    public class UpdateSinavBolumValidator : AbstractValidator<UpdateSinavBolumCommand>
    {
        public UpdateSinavBolumValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Isim).NotEmpty();
            RuleFor(x => x.SinavId).GreaterThan(0);
        }
    }
}
