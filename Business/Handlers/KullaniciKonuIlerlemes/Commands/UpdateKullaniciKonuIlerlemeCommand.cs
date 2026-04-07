
using Business.Constants;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Core.Aspects.Autofac.Validation;
using Business.Handlers.KullaniciKonuIlerlemes.ValidationRules;


namespace Business.Handlers.KullaniciKonuIlerlemes.Commands
{


    public class UpdateKullaniciKonuIlerlemeCommand : IRequest<IResult>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int KonuId { get; set; }
        public Core.Enums.IlerlemeDurumu Durum { get; set; }

        public class UpdateKullaniciKonuIlerlemeCommandHandler : IRequestHandler<UpdateKullaniciKonuIlerlemeCommand, IResult>
        {
            private readonly IKullaniciKonuIlerlemeRepository _kullaniciKonuIlerlemeRepository;
            private readonly IMediator _mediator;

            public UpdateKullaniciKonuIlerlemeCommandHandler(IKullaniciKonuIlerlemeRepository kullaniciKonuIlerlemeRepository, IMediator mediator)
            {
                _kullaniciKonuIlerlemeRepository = kullaniciKonuIlerlemeRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateKullaniciKonuIlerlemeValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateKullaniciKonuIlerlemeCommand request, CancellationToken cancellationToken)
            {
                var isThereKullaniciKonuIlerlemeRecord = await _kullaniciKonuIlerlemeRepository.GetAsync(u => u.Id == request.Id);


                isThereKullaniciKonuIlerlemeRecord.UserId = request.UserId;
                isThereKullaniciKonuIlerlemeRecord.KonuId = request.KonuId;
                isThereKullaniciKonuIlerlemeRecord.Durum = request.Durum;


                _kullaniciKonuIlerlemeRepository.Update(isThereKullaniciKonuIlerlemeRecord);
                await _kullaniciKonuIlerlemeRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}

