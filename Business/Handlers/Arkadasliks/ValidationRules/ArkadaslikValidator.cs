
using Business.Handlers.Arkadasliks.Commands;
using FluentValidation;

namespace Business.Handlers.Arkadasliks.ValidationRules
{

    public class CreateArkadaslikValidator : AbstractValidator<CreateArkadaslikCommand>
    {
        public CreateArkadaslikValidator()
        {
            RuleFor(x => x.UserIdKucuk).GreaterThan(0);
            RuleFor(x => x.UserIdBuyuk).GreaterThan(0);
        }
    }
    public class UpdateArkadaslikValidator : AbstractValidator<UpdateArkadaslikCommand>
    {
        public UpdateArkadaslikValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.UserIdKucuk).GreaterThan(0);
            RuleFor(x => x.UserIdBuyuk).GreaterThan(0);
        }
    }
}
