using Business.Handlers.Authorizations.Commands;
using FluentValidation;

namespace Business.Handlers.Authorizations.ValidationRules
{
    public class RequestEmailVerificationValidator : AbstractValidator<RequestEmailVerificationCommand>
    {
        public RequestEmailVerificationValidator()
        {
            RuleFor(p => p.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(50);
        }
    }
}
