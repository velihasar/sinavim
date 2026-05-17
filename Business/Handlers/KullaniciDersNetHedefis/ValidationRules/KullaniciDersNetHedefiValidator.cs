
using Business.Handlers.KullaniciDersNetHedefis.Commands;
using FluentValidation;

namespace Business.Handlers.KullaniciDersNetHedefis.ValidationRules
{

    public class CreateKullaniciDersNetHedefiValidator : AbstractValidator<CreateKullaniciDersNetHedefiCommand>
    {
        public CreateKullaniciDersNetHedefiValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.DersId).NotEmpty();
            RuleFor(x => x.HedefNet).NotEmpty();

        }
    }
    public class UpdateKullaniciDersNetHedefiValidator : AbstractValidator<UpdateKullaniciDersNetHedefiCommand>
    {
        public UpdateKullaniciDersNetHedefiValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.DersId).NotEmpty();
            RuleFor(x => x.HedefNet).NotEmpty();

        }
    }

    public class UpsertMyKullaniciDersNetHedefiValidator
        : AbstractValidator<UpsertMyKullaniciDersNetHedefiCommand>
    {
        public UpsertMyKullaniciDersNetHedefiValidator()
        {
            RuleFor(x => x.DersId).GreaterThan(0);
            RuleFor(x => x.HedefNet).GreaterThanOrEqualTo(0).LessThanOrEqualTo(500);
        }
    }
}