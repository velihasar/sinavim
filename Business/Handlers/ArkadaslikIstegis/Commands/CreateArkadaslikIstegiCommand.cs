
using Business.BusinessAspects;
using Business.Constants;
using Business.Helpers;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete.Project;
using Core.Enums;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.ArkadaslikIstegis.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateArkadaslikIstegiCommand : IRequest<IResult>
    {

        public int GonderenUserId { get; set; }
        public int HedefUserId { get; set; }
        public string KullanilanDavetKodu { get; set; }
        public System.DateTime? YanitTarihi { get; set; }


        public class CreateArkadaslikIstegiCommandHandler : IRequestHandler<CreateArkadaslikIstegiCommand, IResult>
        {
            private readonly IArkadaslikIstegiRepository _arkadaslikIstegiRepository;
            private readonly IMediator _mediator;
            public CreateArkadaslikIstegiCommandHandler(IArkadaslikIstegiRepository arkadaslikIstegiRepository, IMediator mediator)
            {
                _arkadaslikIstegiRepository = arkadaslikIstegiRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(ValidationRules.CreateArkadaslikIstegiValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateArkadaslikIstegiCommand request, CancellationToken cancellationToken)
            {
                if (request.GonderenUserId == request.HedefUserId)
                {
                    return new ErrorResult("Gönderen ve hedef kullanıcı aynı olamaz.");
                }

                if (await ArkadaslikDomainHelper.HasPendingRequestBetweenAsync(
                        _arkadaslikIstegiRepository,
                        request.GonderenUserId,
                        request.HedefUserId,
                        cancellationToken))
                {
                    return new ErrorResult(Messages.NameAlreadyExist);
                }

                var actorId = UserInfoExtensions.GetUserId();
                var createdBy = actorId > 0 ? actorId : request.GonderenUserId;
                var now = DateTimeExtensions.NowForNpgsqlTimestamp();

                var addedArkadaslikIstegi = new ArkadaslikIstegi
                {
                    GonderenUserId = request.GonderenUserId,
                    HedefUserId = request.HedefUserId,
                    KullanilanDavetKodu = request.KullanilanDavetKodu,
                    Durum = ArkadaslikIstekDurumu.Beklemede,
                    OlusturulmaTarihi = now,
                    YanitTarihi = request.YanitTarihi,
                    CreatedBy = createdBy,
                    CreatedDate = now,
                    IsActive = true,
                };

                _arkadaslikIstegiRepository.Add(addedArkadaslikIstegi);
                await _arkadaslikIstegiRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}
