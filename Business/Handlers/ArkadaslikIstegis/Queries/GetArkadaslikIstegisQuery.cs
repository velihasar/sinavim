
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

namespace Business.Handlers.ArkadaslikIstegis.Queries
{

    public class GetArkadaslikIstegisQuery : IRequest<IDataResult<IEnumerable<ArkadaslikIstegi>>>
    {
        public class GetArkadaslikIstegisQueryHandler : IRequestHandler<GetArkadaslikIstegisQuery, IDataResult<IEnumerable<ArkadaslikIstegi>>>
        {
            private readonly IArkadaslikIstegiRepository _arkadaslikIstegiRepository;
            private readonly IMediator _mediator;

            public GetArkadaslikIstegisQueryHandler(IArkadaslikIstegiRepository arkadaslikIstegiRepository, IMediator mediator)
            {
                _arkadaslikIstegiRepository = arkadaslikIstegiRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<ArkadaslikIstegi>>> Handle(GetArkadaslikIstegisQuery request, CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<ArkadaslikIstegi>>(await _arkadaslikIstegiRepository.GetListAsync());
            }
        }
    }
}