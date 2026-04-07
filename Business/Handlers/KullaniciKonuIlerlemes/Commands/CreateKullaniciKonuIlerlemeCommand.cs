
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Business.Handlers.KullaniciKonuIlerlemes.ValidationRules;
using Core.Entities.Concrete.Project;

namespace Business.Handlers.KullaniciKonuIlerlemes.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateKullaniciKonuIlerlemeCommand : IRequest<IResult>
    {

        public int UserId { get; set; }
        public int KonuId { get; set; }
        public Core.Enums.IlerlemeDurumu Durum { get; set; }


        public class CreateKullaniciKonuIlerlemeCommandHandler : IRequestHandler<CreateKullaniciKonuIlerlemeCommand, IResult>
        {
            private readonly IKullaniciKonuIlerlemeRepository _kullaniciKonuIlerlemeRepository;
            private readonly IMediator _mediator;
            public CreateKullaniciKonuIlerlemeCommandHandler(IKullaniciKonuIlerlemeRepository kullaniciKonuIlerlemeRepository, IMediator mediator)
            {
                _kullaniciKonuIlerlemeRepository = kullaniciKonuIlerlemeRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateKullaniciKonuIlerlemeValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateKullaniciKonuIlerlemeCommand request, CancellationToken cancellationToken)
            {
                var isThereKullaniciKonuIlerlemeRecord = _kullaniciKonuIlerlemeRepository.Query().Any(u => u.UserId == request.UserId);

                if (isThereKullaniciKonuIlerlemeRecord == true)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var addedKullaniciKonuIlerleme = new KullaniciKonuIlerleme
                {
                    UserId = request.UserId,
                    KonuId = request.KonuId,
                    Durum = request.Durum,

                };

                _kullaniciKonuIlerlemeRepository.Add(addedKullaniciKonuIlerleme);
                await _kullaniciKonuIlerlemeRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}