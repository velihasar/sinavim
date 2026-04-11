
using Business.Handlers.Derses.Commands;
using FluentValidation;

namespace Business.Handlers.Derses.ValidationRules
{

    public class CreateDersValidator : AbstractValidator<CreateDersCommand>
    {
        public CreateDersValidator()
        {
            RuleFor(x => x.Ad).NotEmpty();
            RuleFor(x => x.SinavId).NotEmpty();
            RuleFor(x => x.SiraNo).GreaterThanOrEqualTo(0);
            RuleFor(x => x.IkonAnahtari).MaximumLength(128).When(x => !string.IsNullOrEmpty(x.IkonAnahtari));
        }
    }
    public class UpdateDersValidator : AbstractValidator<UpdateDersCommand>
    {
        public UpdateDersValidator()
        {
            RuleFor(x => x.Ad).NotEmpty();
            RuleFor(x => x.SinavId).NotEmpty();
            RuleFor(x => x.SiraNo).GreaterThanOrEqualTo(0);
            RuleFor(x => x.IkonAnahtari).MaximumLength(128).When(x => !string.IsNullOrEmpty(x.IkonAnahtari));
        }
    }
}