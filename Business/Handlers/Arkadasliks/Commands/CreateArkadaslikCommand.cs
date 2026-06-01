
using Business.BusinessAspects;
using Business.Constants;
using Business.Helpers;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete.Project;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Arkadasliks.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateArkadaslikCommand : IRequest<IResult>
    {

        public int UserIdKucuk { get; set; }
        public int UserIdBuyuk { get; set; }
        public int? ArkadaslikIstegiId { get; set; }


        public class CreateArkadaslikCommandHandler : IRequestHandler<CreateArkadaslikCommand, IResult>
        {
            private readonly IArkadaslikRepository _arkadaslikRepository;
            private readonly IMediator _mediator;
            public CreateArkadaslikCommandHandler(IArkadaslikRepository arkadaslikRepository, IMediator mediator)
            {
                _arkadaslikRepository = arkadaslikRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(ValidationRules.CreateArkadaslikValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateArkadaslikCommand request, CancellationToken cancellationToken)
            {
                var (kucuk, buyuk) = ArkadaslikPairHelper.Order(request.UserIdKucuk, request.UserIdBuyuk);
                if (kucuk == buyuk)
                {
                    return new ErrorResult("Kullanıcı çifti geçersiz.");
                }

                var isThereArkadaslikRecord = await _arkadaslikRepository.Query()
                    .AnyAsync(
                        u => u.UserIdKucuk == kucuk && u.UserIdBuyuk == buyuk,
                        cancellationToken);

                if (isThereArkadaslikRecord)
                {
                    return new ErrorResult(Messages.NameAlreadyExist);
                }

                var actorId = UserInfoExtensions.GetUserId();
                var createdBy = actorId > 0 ? actorId : kucuk;
                var now = DateTimeExtensions.NowForNpgsqlTimestamp();

                var addedArkadaslik = new Arkadaslik
                {
                    UserIdKucuk = kucuk,
                    UserIdBuyuk = buyuk,
                    ArkadaslikIstegiId = request.ArkadaslikIstegiId,
                    OlusturulmaTarihi = now,
                    CreatedBy = createdBy,
                    CreatedDate = now,
                    IsActive = true,
                };

                _arkadaslikRepository.Add(addedArkadaslik);
                await _arkadaslikRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}
