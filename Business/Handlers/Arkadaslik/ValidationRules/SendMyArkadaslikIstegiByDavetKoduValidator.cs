using Business.Handlers.ArkadaslikApp.Commands;
using FluentValidation;

namespace Business.Handlers.ArkadaslikApp.ValidationRules
{
    public class SendMyArkadaslikIstegiByDavetKoduValidator
        : AbstractValidator<SendMyArkadaslikIstegiByDavetKoduCommand>
    {
        public SendMyArkadaslikIstegiByDavetKoduValidator()
        {
            RuleFor(x => x.DavetKodu).NotEmpty().MaximumLength(16);
        }
    }
}
