
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
using Business.Handlers.Sinavs.ValidationRules;
using Core.Entities.Dtos.Project.SinavDtos;
using Core.Extensions;


namespace Business.Handlers.Sinavs.Commands
{


    public class UpdateSinavCommand : IRequest<IDataResult<UpdateSinavDto>>
    {
        public int Id { get; set; }
        public string KısaAd { get; set; }
        public string Ad { get; set; }
        public string? Aciklama { get; set; }
        public System.DateTime Tarih { get; set; }
        public int Index { get; set; }
        public int DogruyuGoturenYanlisSay { get; set; }

        public class UpdateSinavCommandHandler : IRequestHandler<UpdateSinavCommand, IDataResult<UpdateSinavDto>>
        {
            private readonly ISinavRepository _sinavRepository;
            private readonly IMediator _mediator;

            public UpdateSinavCommandHandler(ISinavRepository sinavRepository, IMediator mediator)
            {
                _sinavRepository = sinavRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateSinavValidator), Priority = 1)]
            //[CacheRemoveAspect("Get")]
            //[LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<UpdateSinavDto>> Handle(UpdateSinavCommand request, CancellationToken cancellationToken)
            {
                var isThereSinavRecord = await _sinavRepository.GetAsync(u => u.Id == request.Id);

                if(isThereSinavRecord == null)
                {
                    return new ErrorDataResult<UpdateSinavDto>("Kayıt bulunamadı");
                }

                isThereSinavRecord.KısaAd = request.KısaAd;
                isThereSinavRecord.Ad = request.Ad;
                isThereSinavRecord.Aciklama = request.Aciklama;
                isThereSinavRecord.Tarih = request.Tarih;
                isThereSinavRecord.Index = request.Index;
                isThereSinavRecord.DogruyuGoturenYanlisSay = request.DogruyuGoturenYanlisSay;
                isThereSinavRecord.UpdatedBy = UserInfoExtensions.GetUserId();
                isThereSinavRecord.UpdatedDate = System.DateTime.Now;


                _sinavRepository.Update(isThereSinavRecord);
                await _sinavRepository.SaveChangesAsync();

                var sinavDto = new UpdateSinavDto
                {
                    Id = isThereSinavRecord.Id,
                    KısaAd = isThereSinavRecord.KısaAd,
                    Ad = isThereSinavRecord.Ad,
                    Aciklama = isThereSinavRecord.Aciklama,
                    Tarih = isThereSinavRecord.Tarih,
                    Index = isThereSinavRecord.Index,
                    DogruyuGoturenYanlisSay = isThereSinavRecord.DogruyuGoturenYanlisSay
                };

                return new SuccessDataResult<UpdateSinavDto>(sinavDto, Messages.Updated);
            }
        }
    }
}

