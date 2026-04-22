using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.KullaniciGunlukSoruCozumus.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.Project.KullaniciGunlukSoruCozumuDtos;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.KullaniciGunlukSoruCozumus.Commands
{
    public class CreateKullaniciGunlukSoruCozumuCommand : CreateKullaniciGunlukSoruCozumuDto, IRequest<IResult>
    {
        public class CreateKullaniciGunlukSoruCozumuCommandHandler : IRequestHandler<CreateKullaniciGunlukSoruCozumuCommand, IResult>
        {
            private readonly IKullaniciGunlukSoruCozumuRepository _kullaniciGunlukSoruCozumuRepository;
            private readonly IMediator _mediator;

            public CreateKullaniciGunlukSoruCozumuCommandHandler(
                IKullaniciGunlukSoruCozumuRepository kullaniciGunlukSoruCozumuRepository,
                IMediator mediator)
            {
                _kullaniciGunlukSoruCozumuRepository = kullaniciGunlukSoruCozumuRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateKullaniciGunlukSoruCozumuValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateKullaniciGunlukSoruCozumuCommand request, CancellationToken cancellationToken)
            {
                var isThereKullaniciGunlukSoruCozumuRecord =
                    _kullaniciGunlukSoruCozumuRepository.Query().Any(u => u.UserId == request.UserId);

                if (isThereKullaniciGunlukSoruCozumuRecord)
                {
                    return new ErrorResult(Messages.NameAlreadyExist);
                }

                var addedKullaniciGunlukSoruCozumu = new KullaniciGunlukSoruCozumu
                {
                    UserId = request.UserId,
                    Tarih = request.Tarih.ToNpgsqlDateOnly(),
                    CozulenSoruSayisi = request.CozulenSoruSayisi,
                };

                _kullaniciGunlukSoruCozumuRepository.Add(addedKullaniciGunlukSoruCozumu);
                await _kullaniciGunlukSoruCozumuRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}
