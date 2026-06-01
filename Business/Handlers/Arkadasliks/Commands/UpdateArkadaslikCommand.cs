
using Business.BusinessAspects;
using Business.Constants;
using Business.Helpers;
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
using Business.Handlers.Arkadasliks.ValidationRules;


namespace Business.Handlers.Arkadasliks.Commands
{


    public class UpdateArkadaslikCommand : IRequest<IResult>
    {
        public int Id { get; set; }
        public int UserIdKucuk { get; set; }
        public int UserIdBuyuk { get; set; }
        public int? ArkadaslikIstegiId { get; set; }
        public bool? IsActive { get; set; }

        public class UpdateArkadaslikCommandHandler : IRequestHandler<UpdateArkadaslikCommand, IResult>
        {
            private readonly IArkadaslikRepository _arkadaslikRepository;
            private readonly IMediator _mediator;

            public UpdateArkadaslikCommandHandler(IArkadaslikRepository arkadaslikRepository, IMediator mediator)
            {
                _arkadaslikRepository = arkadaslikRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateArkadaslikValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateArkadaslikCommand request, CancellationToken cancellationToken)
            {
                var isThereArkadaslikRecord = await _arkadaslikRepository.GetAsync(u => u.Id == request.Id);

                if (isThereArkadaslikRecord == null)
                {
                    return new ErrorResult(Messages.RecordNotFound);
                }

                var (kucuk, buyuk) = ArkadaslikPairHelper.Order(request.UserIdKucuk, request.UserIdBuyuk);
                var actorId = UserInfoExtensions.GetUserId();
                var now = DateTimeExtensions.NowForNpgsqlTimestamp();

                isThereArkadaslikRecord.UserIdKucuk = kucuk;
                isThereArkadaslikRecord.UserIdBuyuk = buyuk;
                isThereArkadaslikRecord.ArkadaslikIstegiId = request.ArkadaslikIstegiId;
                isThereArkadaslikRecord.IsActive = request.IsActive ?? isThereArkadaslikRecord.IsActive;
                isThereArkadaslikRecord.UpdatedBy = actorId > 0 ? actorId : kucuk;
                isThereArkadaslikRecord.UpdatedDate = now;


                _arkadaslikRepository.Update(isThereArkadaslikRecord);
                await _arkadaslikRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}
