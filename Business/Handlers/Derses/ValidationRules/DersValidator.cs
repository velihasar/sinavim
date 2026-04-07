
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

        }
    }
    public class UpdateDersValidator : AbstractValidator<UpdateDersCommand>
    {
        public UpdateDersValidator()
        {
            RuleFor(x => x.Ad).NotEmpty();
            RuleFor(x => x.SinavId).NotEmpty();

        }
    }
}