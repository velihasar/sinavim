
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

namespace Business.Handlers.Konus.Queries
{

    public class GetKonusQuery : IRequest<IDataResult<IEnumerable<Konu>>>
    {
        public class GetKonusQueryHandler : IRequestHandler<GetKonusQuery, IDataResult<IEnumerable<Konu>>>
        {
            private readonly IKonuRepository _konuRepository;
            private readonly IMediator _mediator;

            public GetKonusQueryHandler(IKonuRepository konuRepository, IMediator mediator)
            {
                _konuRepository = konuRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<Konu>>> Handle(GetKonusQuery request, CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<Konu>>(await _konuRepository.GetListAsync());
            }
        }
    }
}