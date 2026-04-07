
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
using Core.Entities.Dtos.Project.DenemeSinaviDtos;


namespace Business.Handlers.DenemeSinavis.Queries
{
    public class GetDenemeSinaviQuery : IRequest<IDataResult<DenemeSinaviDto>>
    {
        public int Id { get; set; }

        public class GetDenemeSinaviQueryHandler : IRequestHandler<GetDenemeSinaviQuery, IDataResult<DenemeSinaviDto>>
        {
            private readonly IDenemeSinaviRepository _denemeSinaviRepository;
            private readonly IMediator _mediator;

            public GetDenemeSinaviQueryHandler(IDenemeSinaviRepository denemeSinaviRepository, IMediator mediator)
            {
                _denemeSinaviRepository = denemeSinaviRepository;
                _mediator = mediator;
            }
            //[LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<DenemeSinaviDto>> Handle(GetDenemeSinaviQuery request, CancellationToken cancellationToken)
            {
                var denemeSinavi = await _denemeSinaviRepository.GetAsync(p => p.Id == request.Id);
                if (denemeSinavi == null)
                {
                    return new ErrorDataResult<DenemeSinaviDto>(null, "Kayıt bulunamadı");
                }
                var dto = new DenemeSinaviDto
                {
                    Id = denemeSinavi.Id,
                    Ad = denemeSinavi.Ad,
                    Aciklama = denemeSinavi.Aciklama,
                    UserId = denemeSinavi.UserId,
                    SinavId = denemeSinavi.SinavId,
                    Tarih = denemeSinavi.Tarih
                };
                return new SuccessDataResult<DenemeSinaviDto>(dto);
            }
        }
    }
}
