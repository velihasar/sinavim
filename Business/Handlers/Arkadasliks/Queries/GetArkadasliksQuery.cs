
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
using ArkadaslikEntity = Core.Entities.Concrete.Project.Arkadaslik;

namespace Business.Handlers.Arkadasliks.Queries
{

    public class GetArkadasliksQuery : IRequest<IDataResult<IEnumerable<ArkadaslikEntity>>>
    {
        public class GetArkadasliksQueryHandler : IRequestHandler<GetArkadasliksQuery, IDataResult<IEnumerable<ArkadaslikEntity>>>
        {
            private readonly IArkadaslikRepository _arkadaslikRepository;
            private readonly IMediator _mediator;

            public GetArkadasliksQueryHandler(IArkadaslikRepository arkadaslikRepository, IMediator mediator)
            {
                _arkadaslikRepository = arkadaslikRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<ArkadaslikEntity>>> Handle(GetArkadasliksQuery request, CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<ArkadaslikEntity>>(await _arkadaslikRepository.GetListAsync());
            }
        }
    }
}