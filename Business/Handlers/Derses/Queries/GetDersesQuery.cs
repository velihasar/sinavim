
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
using Core.Entities.Dtos.Project.DersDtos;
using Microsoft.EntityFrameworkCore;

namespace Business.Handlers.Derses.Queries
{

    public class GetDersesQuery : IRequest<IDataResult<IEnumerable<DersListDto>>>
    {
        public class GetDersesQueryHandler : IRequestHandler<GetDersesQuery, IDataResult<IEnumerable<DersListDto>>>
        {
            private readonly IDersRepository _dersRepository;
            private readonly IMediator _mediator;

            public GetDersesQueryHandler(IDersRepository dersRepository, IMediator mediator)
            {
                _dersRepository = dersRepository;
                _mediator = mediator;
            }

            //[PerformanceAspect(5)]
            //[CacheAspect(10)]
            //[LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<DersListDto>>> Handle(GetDersesQuery request, CancellationToken cancellationToken)
            {
                var list = await _dersRepository.Query()
                    .Include(d => d.SinavBolum)
                    .OrderBy(d => d.SiraNo)
                    .ThenBy(d => d.Id)
                    .ToListAsync(cancellationToken);
                var dtoList = list.Select(d => new DersListDto
                {
                    Id = d.Id,
                    Ad = d.Ad,
                    IkonAnahtari = d.IkonAnahtari,
                    SiraNo = d.SiraNo,
                    SinavId = d.SinavId,
                    SinavBolumId = d.SinavBolumId,
                    SinavBolumIsim = d.SinavBolum?.Isim
                });

                return new SuccessDataResult<IEnumerable<DersListDto>>(dtoList);
            }
        }
    }
}