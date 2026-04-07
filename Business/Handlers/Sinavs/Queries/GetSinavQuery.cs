
using Business.BusinessAspects;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.Project.SinavDtos;


namespace Business.Handlers.Sinavs.Queries
{
    public class GetSinavQuery : IRequest<IDataResult<SinavDto>>
    {
        public int Id { get; set; }

        public class GetSinavQueryHandler : IRequestHandler<GetSinavQuery, IDataResult<SinavDto>>
        {
            private readonly ISinavRepository _sinavRepository;
            private readonly IMediator _mediator;

            public GetSinavQueryHandler(ISinavRepository sinavRepository, IMediator mediator)
            {
                _sinavRepository = sinavRepository;
                _mediator = mediator;
            }
            //[LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<SinavDto>> Handle(GetSinavQuery request, CancellationToken cancellationToken)
            {
                var sinav = await _sinavRepository.GetAsync(p => p.Id == request.Id);
                
                if(sinav == null)
                {
                    return new ErrorDataResult<SinavDto>(null, "Kayıt bulunamadı");
                }

                var sinavDto = new SinavDto
                {
                    Id = sinav.Id,
                    KısaAd = sinav.KısaAd,
                    Ad = sinav.Ad,
                    Tarih = sinav.Tarih,
                    DogruyuGoturenYanlisSay = sinav.DogruyuGoturenYanlisSay
                };

                return new SuccessDataResult<SinavDto>(sinavDto);
            }
        }
    }
}
