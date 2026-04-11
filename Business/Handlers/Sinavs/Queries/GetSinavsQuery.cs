
using Business.BusinessAspects;
using Core.Aspects.Autofac.Performance;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Aspects.Autofac.Caching;
using Core.Entities.Concrete.Project;
using System.Linq;
using Core.Entities.Dtos.Project.SinavDtos;
using Microsoft.EntityFrameworkCore;

namespace Business.Handlers.Sinavs.Queries
{

    public class GetSinavsQuery : IRequest<IDataResult<IEnumerable<SinavListDto>>>
    {
        public class GetSinavsQueryHandler : IRequestHandler<GetSinavsQuery, IDataResult<IEnumerable<SinavListDto>>>
        {
            private readonly ISinavRepository _sinavRepository;
            private readonly IMediator _mediator;

            public GetSinavsQueryHandler(ISinavRepository sinavRepository, IMediator mediator)
            {
                _sinavRepository = sinavRepository;
                _mediator = mediator;
            }

            //[PerformanceAspect(5)]
            //[CacheAspect(10)]
            //[LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<SinavListDto>>> Handle(GetSinavsQuery request, CancellationToken cancellationToken)
            {
                var sinavs = await _sinavRepository.Query()
                    .OrderBy(s => s.SiraNo)
                    .ThenBy(s => s.Id)
                    .ToListAsync(cancellationToken);
                var sinavDtos = sinavs.Select(s => new SinavListDto
                {
                    Id = s.Id,
                    KısaAd = s.KısaAd,
                    Ad = s.Ad,
                    Aciklama = s.Aciklama,
                    Tarih = s.Tarih,
                    SiraNo = s.SiraNo,
                    DogruyuGoturenYanlisSay = s.DogruyuGoturenYanlisSay
                });

                return new SuccessDataResult<IEnumerable<SinavListDto>>(sinavDtos);
            }
        }
    }
}