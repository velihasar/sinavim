
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
using Core.Entities.Dtos.Project.DenemeSinavSonucuDtos;

namespace Business.Handlers.DenemeSinavSonucus.Queries
{
    public class GetDenemeSinavSonucuQuery : IRequest<IDataResult<DenemeSinavSonucuDto>>
    {
        public int Id { get; set; }

        public class GetDenemeSinavSonucuQueryHandler : IRequestHandler<GetDenemeSinavSonucuQuery, IDataResult<DenemeSinavSonucuDto>>
        {
            private readonly IDenemeSinavSonucuRepository _denemeSinavSonucuRepository;
            private readonly IMediator _mediator;

            public GetDenemeSinavSonucuQueryHandler(IDenemeSinavSonucuRepository denemeSinavSonucuRepository, IMediator mediator)
            {
                _denemeSinavSonucuRepository = denemeSinavSonucuRepository;
                _mediator = mediator;
            }
            //[LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<DenemeSinavSonucuDto>> Handle(GetDenemeSinavSonucuQuery request, CancellationToken cancellationToken)
            {
                var denemeSinavSonucu = await _denemeSinavSonucuRepository.GetAsync(p => p.Id == request.Id);
                
                if(denemeSinavSonucu == null)
                {
                    return new ErrorDataResult<DenemeSinavSonucuDto>(null, "Kayıt bulunamadı");
                }

                var dto = new DenemeSinavSonucuDto
                {
                    Id = denemeSinavSonucu.Id,
                    DenemeSinaviId = denemeSinavSonucu.DenemeSinaviId,
                    DersId = denemeSinavSonucu.DersId,
                    DogruSayisi = denemeSinavSonucu.DogruSayisi,
                    YanlisSayisi = denemeSinavSonucu.YanlisSayisi,
                    BosSayisi = denemeSinavSonucu.BosSayisi,
                    ToplamNet = denemeSinavSonucu.ToplamNet
                };

                return new SuccessDataResult<DenemeSinavSonucuDto>(dto);
            }
        }
    }
}
