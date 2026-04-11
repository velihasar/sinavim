
using Business.Handlers.Motivasyons.Commands;
using FluentValidation;

namespace Business.Handlers.Motivasyons.ValidationRules
{

    public class CreateMotivasyonValidator : AbstractValidator<CreateMotivasyonCommand>
    {
        public CreateMotivasyonValidator()
        {
            RuleFor(x => x.Kelime).NotEmpty();

        }
    }
    public class UpdateMotivasyonValidator : AbstractValidator<UpdateMotivasyonCommand>
    {
        public UpdateMotivasyonValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Kelime).NotEmpty();
        }
    }
}