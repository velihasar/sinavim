
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

namespace Business.Handlers.DenemeSinavSonucus.Queries
{

    public class GetDenemeSinavSonucusQuery : IRequest<IDataResult<IEnumerable<DenemeSinavSonucu>>>
    {
        public class GetDenemeSinavSonucusQueryHandler : IRequestHandler<GetDenemeSinavSonucusQuery, IDataResult<IEnumerable<DenemeSinavSonucu>>>
        {
            private readonly IDenemeSinavSonucuRepository _denemeSinavSonucuRepository;
            private readonly IMediator _mediator;

            public GetDenemeSinavSonucusQueryHandler(IDenemeSinavSonucuRepository denemeSinavSonucuRepository, IMediator mediator)
            {
                _denemeSinavSonucuRepository = denemeSinavSonucuRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<DenemeSinavSonucu>>> Handle(GetDenemeSinavSonucusQuery request, CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<DenemeSinavSonucu>>(await _denemeSinavSonucuRepository.GetListAsync());
            }
        }
    }
}