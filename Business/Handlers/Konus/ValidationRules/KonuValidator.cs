
using Business.Handlers.Konus.Commands;
using FluentValidation;

namespace Business.Handlers.Konus.ValidationRules
{

    public class CreateKonuValidator : AbstractValidator<CreateKonuCommand>
    {
        public CreateKonuValidator()
        {
            RuleFor(x => x.Ad).NotEmpty();
            RuleFor(x => x.SiraNo).NotEmpty();
            RuleFor(x => x.DersId).NotEmpty();

        }
    }
    public class UpdateKonuValidator : AbstractValidator<UpdateKonuCommand>
    {
        public UpdateKonuValidator()
        {
            RuleFor(x => x.Ad).NotEmpty();
            RuleFor(x => x.SiraNo).NotEmpty();
            RuleFor(x => x.DersId).NotEmpty();

        }
    }
}