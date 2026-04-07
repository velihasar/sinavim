
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
using Business.Handlers.KullaniciSinavs.ValidationRules;
using Core.Entities.Dtos.Project.KullaniciSinavDtos;
using Core.Extensions;

namespace Business.Handlers.KullaniciSinavs.Commands
{


    public class UpdateKullaniciSinavCommand : IRequest<IDataResult<UpdateKullaniciSinavDto>>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SinavId { get; set; }
        public int HedefPuan { get; set; }

        public class UpdateKullaniciSinavCommandHandler : IRequestHandler<UpdateKullaniciSinavCommand, IDataResult<UpdateKullaniciSinavDto>>
        {
            private readonly IKullaniciSinavRepository _kullaniciSinavRepository;
            private readonly IMediator _mediator;

            public UpdateKullaniciSinavCommandHandler(IKullaniciSinavRepository kullaniciSinavRepository, IMediator mediator)
            {
                _kullaniciSinavRepository = kullaniciSinavRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateKullaniciSinavValidator), Priority = 1)]
            //[CacheRemoveAspect("Get")]
            //[LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<UpdateKullaniciSinavDto>> Handle(UpdateKullaniciSinavCommand request, CancellationToken cancellationToken)
            {
                var isThereKullaniciSinavRecord = await _kullaniciSinavRepository.GetAsync(u => u.Id == request.Id);

                if(isThereKullaniciSinavRecord == null)
                {
                    return new ErrorDataResult<UpdateKullaniciSinavDto>("Kayıt bulunamadı");
                }

                isThereKullaniciSinavRecord.UserId = request.UserId;
                isThereKullaniciSinavRecord.SinavId = request.SinavId;
                isThereKullaniciSinavRecord.HedefPuan = request.HedefPuan;
                isThereKullaniciSinavRecord.UpdatedBy = UserInfoExtensions.GetUserId();
                isThereKullaniciSinavRecord.UpdatedDate = System.DateTime.Now;


                _kullaniciSinavRepository.Update(isThereKullaniciSinavRecord);
                await _kullaniciSinavRepository.SaveChangesAsync();
                
                var dto = new UpdateKullaniciSinavDto
                {
                    Id = isThereKullaniciSinavRecord.Id,
                    UserId = isThereKullaniciSinavRecord.UserId,
                    SinavId = isThereKullaniciSinavRecord.SinavId,
                    HedefPuan = isThereKullaniciSinavRecord.HedefPuan
                };

                return new SuccessDataResult<UpdateKullaniciSinavDto>(dto, Messages.Updated);
            }
        }
    }
}

