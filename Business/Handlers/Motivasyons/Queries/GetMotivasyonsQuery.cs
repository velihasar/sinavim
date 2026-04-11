
using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.Project.MotivasyonDtos;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Motivasyons.Queries
{
    public class GetMotivasyonsQuery : IRequest<IDataResult<IEnumerable<MotivasyonListDto>>>
    {
        public class GetMotivasyonsQueryHandler : IRequestHandler<GetMotivasyonsQuery, IDataResult<IEnumerable<MotivasyonListDto>>>
        {
            private readonly IMotivasyonRepository _motivasyonRepository;
            private readonly IMediator _mediator;

            public GetMotivasyonsQueryHandler(IMotivasyonRepository motivasyonRepository, IMediator mediator)
            {
                _motivasyonRepository = motivasyonRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<MotivasyonListDto>>> Handle(
                GetMotivasyonsQuery request,
                CancellationToken cancellationToken)
            {
                var list = await _motivasyonRepository.GetListAsync();
                var dtoList = list.Select(m => new MotivasyonListDto
                {
                    Id = m.Id,
                    Kelime = m.Kelime,
                });

                return new SuccessDataResult<IEnumerable<MotivasyonListDto>>(dtoList);
            }
        }
    }
}
