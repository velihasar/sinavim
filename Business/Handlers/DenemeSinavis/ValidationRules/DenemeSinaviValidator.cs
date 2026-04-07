
using Business.Handlers.DenemeSinavis.Commands;
using FluentValidation;

namespace Business.Handlers.DenemeSinavis.ValidationRules
{

    public class CreateDenemeSinaviValidator : AbstractValidator<CreateDenemeSinaviCommand>
    {
        public CreateDenemeSinaviValidator()
        {
            RuleFor(x => x.Ad).NotEmpty();
            RuleFor(x => x.Aciklama).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.SinavId).NotEmpty();
            RuleFor(x => x.Tarih).NotEmpty();

        }
    }
    public class UpdateDenemeSinaviValidator : AbstractValidator<UpdateDenemeSinaviCommand>
    {
        public UpdateDenemeSinaviValidator()
        {
            RuleFor(x => x.Ad).NotEmpty();
            RuleFor(x => x.Aciklama).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.SinavId).NotEmpty();
            RuleFor(x => x.Tarih).NotEmpty();

        }
    }
}