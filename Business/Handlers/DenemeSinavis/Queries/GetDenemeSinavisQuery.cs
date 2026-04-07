
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

namespace Business.Handlers.DenemeSinavis.Queries
{

    public class GetDenemeSinavisQuery : IRequest<IDataResult<IEnumerable<DenemeSinavi>>>
    {
        public class GetDenemeSinavisQueryHandler : IRequestHandler<GetDenemeSinavisQuery, IDataResult<IEnumerable<DenemeSinavi>>>
        {
            private readonly IDenemeSinaviRepository _denemeSinaviRepository;
            private readonly IMediator _mediator;

            public GetDenemeSinavisQueryHandler(IDenemeSinaviRepository denemeSinaviRepository, IMediator mediator)
            {
                _denemeSinaviRepository = denemeSinaviRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<DenemeSinavi>>> Handle(GetDenemeSinavisQuery request, CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<DenemeSinavi>>(await _denemeSinaviRepository.GetListAsync());
            }
        }
    }
}