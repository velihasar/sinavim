
using Business.Handlers.DenemeSinavSonucus.Commands;
using FluentValidation;

namespace Business.Handlers.DenemeSinavSonucus.ValidationRules
{

    public class CreateDenemeSinavSonucuValidator : AbstractValidator<CreateDenemeSinavSonucuCommand>
    {
        public CreateDenemeSinavSonucuValidator()
        {
            RuleFor(x => x.DersId).NotEmpty();
            RuleFor(x => x.DogruSayisi).NotEmpty();
            RuleFor(x => x.YanlisSayisi).NotEmpty();
            RuleFor(x => x.BosSayisi).NotEmpty();
            RuleFor(x => x.ToplamNet).NotEmpty();

        }
    }
    public class UpdateDenemeSinavSonucuValidator : AbstractValidator<UpdateDenemeSinavSonucuCommand>
    {
        public UpdateDenemeSinavSonucuValidator()
        {
            RuleFor(x => x.DersId).NotEmpty();
            RuleFor(x => x.DogruSayisi).NotEmpty();
            RuleFor(x => x.YanlisSayisi).NotEmpty();
            RuleFor(x => x.BosSayisi).NotEmpty();
            RuleFor(x => x.ToplamNet).NotEmpty();

        }
    }
}