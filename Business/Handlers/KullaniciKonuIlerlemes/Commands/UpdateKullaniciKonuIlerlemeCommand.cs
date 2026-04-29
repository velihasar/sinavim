
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
using Core.Entities.Dtos.Project.KullaniciKonuIlerlemeDtos;
using Core.Extensions;

namespace Business.Handlers.KullaniciKonuIlerlemes.Commands
{


    public class UpdateKullaniciKonuIlerlemeCommand : IRequest<IDataResult<UpdateKullaniciKonuIlerlemeDto>>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int KonuId { get; set; }
        public Core.Enums.IlerlemeDurumu Durum { get; set; }

        public class UpdateKullaniciKonuIlerlemeCommandHandler : IRequestHandler<UpdateKullaniciKonuIlerlemeCommand, IDataResult<UpdateKullaniciKonuIlerlemeDto>>
        {
            private readonly IKullaniciKonuIlerlemeRepository _kullaniciKonuIlerlemeRepository;
            private readonly IMediator _mediator;

            public UpdateKullaniciKonuIlerlemeCommandHandler(IKullaniciKonuIlerlemeRepository kullaniciKonuIlerlemeRepository, IMediator mediator)
            {
                _kullaniciKonuIlerlemeRepository = kullaniciKonuIlerlemeRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateKullaniciKonuIlerlemeValidator), Priority = 1)]
            //[CacheRemoveAspect("Get")]
            //[LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<UpdateKullaniciKonuIlerlemeDto>> Handle(UpdateKullaniciKonuIlerlemeCommand request, CancellationToken cancellationToken)
            {
                var isThereKullaniciKonuIlerlemeRecord = await _kullaniciKonuIlerlemeRepository.GetAsync(u => u.Id == request.Id);

                if(isThereKullaniciKonuIlerlemeRecord == null)
                {
                    return new ErrorDataResult<UpdateKullaniciKonuIlerlemeDto>("Kayıt bulunamadı");
                }

                isThereKullaniciKonuIlerlemeRecord.UserId = request.UserId;
                isThereKullaniciKonuIlerlemeRecord.KonuId = request.KonuId;
                isThereKullaniciKonuIlerlemeRecord.Durum = request.Durum;
                isThereKullaniciKonuIlerlemeRecord.UpdatedBy = UserInfoExtensions.GetUserId();
                isThereKullaniciKonuIlerlemeRecord.UpdatedDate = DateTimeExtensions.NowForNpgsqlTimestamp();


                _kullaniciKonuIlerlemeRepository.Update(isThereKullaniciKonuIlerlemeRecord);
                await _kullaniciKonuIlerlemeRepository.SaveChangesAsync();
                
                var dto = new UpdateKullaniciKonuIlerlemeDto
                {
                    Id = isThereKullaniciKonuIlerlemeRecord.Id,
                    UserId = isThereKullaniciKonuIlerlemeRecord.UserId,
                    KonuId = isThereKullaniciKonuIlerlemeRecord.KonuId,
                    Durum = isThereKullaniciKonuIlerlemeRecord.Durum
                };

                return new SuccessDataResult<UpdateKullaniciKonuIlerlemeDto>(dto, Messages.Updated);
            }
        }
    }
}

