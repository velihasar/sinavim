
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Validation;
using Business.Handlers.ArkadaslikIstegis.ValidationRules;


namespace Business.Handlers.ArkadaslikIstegis.Commands
{


    public class UpdateArkadaslikIstegiCommand : IRequest<IResult>
    {
        public int Id { get; set; }
        public int GonderenUserId { get; set; }
        public int HedefUserId { get; set; }
        public string KullanilanDavetKodu { get; set; }
        public Core.Enums.ArkadaslikIstekDurumu Durum { get; set; }
        public System.DateTime? YanitTarihi { get; set; }

        public class UpdateArkadaslikIstegiCommandHandler : IRequestHandler<UpdateArkadaslikIstegiCommand, IResult>
        {
            private readonly IArkadaslikIstegiRepository _arkadaslikIstegiRepository;
            private readonly IMediator _mediator;

            public UpdateArkadaslikIstegiCommandHandler(IArkadaslikIstegiRepository arkadaslikIstegiRepository, IMediator mediator)
            {
                _arkadaslikIstegiRepository = arkadaslikIstegiRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateArkadaslikIstegiValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateArkadaslikIstegiCommand request, CancellationToken cancellationToken)
            {
                var isThereArkadaslikIstegiRecord = await _arkadaslikIstegiRepository.GetAsync(u => u.Id == request.Id);

                if (isThereArkadaslikIstegiRecord == null)
                {
                    return new ErrorResult(Messages.RecordNotFound);
                }

                var actorId = UserInfoExtensions.GetUserId();
                var now = DateTimeExtensions.NowForNpgsqlTimestamp();

                isThereArkadaslikIstegiRecord.GonderenUserId = request.GonderenUserId;
                isThereArkadaslikIstegiRecord.HedefUserId = request.HedefUserId;
                isThereArkadaslikIstegiRecord.KullanilanDavetKodu = request.KullanilanDavetKodu;
                isThereArkadaslikIstegiRecord.Durum = request.Durum;
                isThereArkadaslikIstegiRecord.YanitTarihi = request.YanitTarihi;
                isThereArkadaslikIstegiRecord.UpdatedBy = actorId > 0 ? actorId : request.GonderenUserId;
                isThereArkadaslikIstegiRecord.UpdatedDate = now;


                _arkadaslikIstegiRepository.Update(isThereArkadaslikIstegiRecord);
                await _arkadaslikIstegiRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}
