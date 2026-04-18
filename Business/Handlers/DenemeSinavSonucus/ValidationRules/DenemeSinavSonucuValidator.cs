
using Business.Handlers.DenemeSinavSonucus.Commands;
using FluentValidation;

namespace Business.Handlers.DenemeSinavSonucus.ValidationRules
{

    public class CreateDenemeSinavSonucuValidator : AbstractValidator<CreateDenemeSinavSonucuCommand>
    {
        public CreateDenemeSinavSonucuValidator()
        {
            RuleFor(x => x.DenemeSinaviId).NotEmpty();
            RuleFor(x => x.DersId).NotEmpty();
            RuleFor(x => x.DogruSayisi).GreaterThanOrEqualTo(0);
            RuleFor(x => x.YanlisSayisi).GreaterThanOrEqualTo(0);
            RuleFor(x => x.BosSayisi).GreaterThanOrEqualTo(0);

        }
    }
    public class UpdateDenemeSinavSonucuValidator : AbstractValidator<UpdateDenemeSinavSonucuCommand>
    {
        public UpdateDenemeSinavSonucuValidator()
        {
            RuleFor(x => x.DenemeSinaviId).NotEmpty();
            RuleFor(x => x.DersId).NotEmpty();
            RuleFor(x => x.DogruSayisi).GreaterThanOrEqualTo(0);
            RuleFor(x => x.YanlisSayisi).GreaterThanOrEqualTo(0);
            RuleFor(x => x.BosSayisi).GreaterThanOrEqualTo(0);

        }
    }
}