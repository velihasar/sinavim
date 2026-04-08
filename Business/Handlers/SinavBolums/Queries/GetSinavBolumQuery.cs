
using Business.BusinessAspects;
using Core.Entities.Dtos.Project.SinavBolumDtos;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.SinavBolums.Queries
{
    public class GetSinavBolumQuery : IRequest<IDataResult<SinavBolumDto>>
    {
        public int Id { get; set; }

        public class GetSinavBolumQueryHandler : IRequestHandler<GetSinavBolumQuery, IDataResult<SinavBolumDto>>
        {
            private readonly ISinavBolumRepository _sinavBolumRepository;
            private readonly IMediator _mediator;

            public GetSinavBolumQueryHandler(ISinavBolumRepository sinavBolumRepository, IMediator mediator)
            {
                _sinavBolumRepository = sinavBolumRepository;
                _mediator = mediator;
            }

            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<SinavBolumDto>> Handle(GetSinavBolumQuery request, CancellationToken cancellationToken)
            {
                var entity = await _sinavBolumRepository.Query()
                    .Include(b => b.Sinav)
                    .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

                if (entity == null)
                    return new ErrorDataResult<SinavBolumDto>(null, "Kayıt bulunamadı");

                var dto = new SinavBolumDto
                {
                    Id = entity.Id,
                    SinavId = entity.SinavId,
                    Isim = entity.Isim,
                    SinavAd = entity.Sinav?.Ad
                };

                return new SuccessDataResult<SinavBolumDto>(dto);
            }
        }
    }
}
