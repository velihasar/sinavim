
using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.Sinavs.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.Project.SinavDtos;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Sinavs.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateSinavCommand : IRequest<IDataResult<CreateSinavDto>>
    {
        public string KısaAd { get; set; }
        public string Ad { get; set; }
        public System.DateTime Tarih { get; set; }
        public int DogruyuGoturenYanlisSay { get; set; }

        public class CreateSinavCommandHandler : IRequestHandler<CreateSinavCommand, IDataResult<CreateSinavDto>>
        {
            private readonly ISinavRepository _sinavRepository;
            private readonly IMediator _mediator;
            public CreateSinavCommandHandler(ISinavRepository sinavRepository, IMediator mediator)
            {
                _sinavRepository = sinavRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateSinavValidator), Priority = 1)]
            //[CacheRemoveAspect("Get")]
            //[LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<CreateSinavDto>> Handle(CreateSinavCommand request, CancellationToken cancellationToken)
            {
                var isThereSinavRecord = _sinavRepository.Query().Any(u => u.Ad == request.Ad);

                if (isThereSinavRecord == true)
                    return new ErrorDataResult<CreateSinavDto>(Messages.NameAlreadyExist);

                var addedSinav = new Sinav
                {
                    KısaAd = request.KısaAd,
                    Ad = request.Ad,
                    Tarih = request.Tarih,
                    DogruyuGoturenYanlisSay = request.DogruyuGoturenYanlisSay,
                    CreatedBy = UserInfoExtensions.GetUserId(),
                    CreatedDate = System.DateTime.Now,
                    IsActive = true
                };

                _sinavRepository.Add(addedSinav);
                await _sinavRepository.SaveChangesAsync();

                var sinavDto = new CreateSinavDto
                {
                    Id = addedSinav.Id,
                    KısaAd = addedSinav.KısaAd,
                    Ad = addedSinav.Ad,
                    Tarih = addedSinav.Tarih,
                    DogruyuGoturenYanlisSay = addedSinav.DogruyuGoturenYanlisSay
                };

                return new SuccessDataResult<CreateSinavDto>(sinavDto, Messages.Added);
            }
        }
    }
}