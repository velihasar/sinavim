
using Business.BusinessAspects;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.Project.SinavBolumDtos;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.SinavBolums.Queries
{
    public class GetSinavBolumsQuery : IRequest<IDataResult<IEnumerable<SinavBolumListDto>>>
    {
        public int? SinavId { get; set; }

        public class GetSinavBolumsQueryHandler : IRequestHandler<GetSinavBolumsQuery, IDataResult<IEnumerable<SinavBolumListDto>>>
        {
            private readonly ISinavBolumRepository _sinavBolumRepository;
            private readonly IMediator _mediator;

            public GetSinavBolumsQueryHandler(ISinavBolumRepository sinavBolumRepository, IMediator mediator)
            {
                _sinavBolumRepository = sinavBolumRepository;
                _mediator = mediator;
            }

            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<SinavBolumListDto>>> Handle(GetSinavBolumsQuery request, CancellationToken cancellationToken)
            {
                IQueryable<SinavBolum> query = _sinavBolumRepository.Query().Include(b => b.Sinav);

                if (request.SinavId.HasValue)
                    query = query.Where(b => b.SinavId == request.SinavId.Value);

                var list = await query.ToListAsync(cancellationToken);
                var dtoList = list.Select(b => new SinavBolumListDto
                {
                    Id = b.Id,
                    SinavId = (int)b.SinavId,
                    Isim = b.Isim,
                    SinavAd = b.Sinav?.Ad
                });

                return new SuccessDataResult<IEnumerable<SinavBolumListDto>>(dtoList);
            }
        }
    }
}
