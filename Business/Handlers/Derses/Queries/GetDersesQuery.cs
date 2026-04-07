
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

namespace Business.Handlers.Derses.Queries
{

    public class GetDersesQuery : IRequest<IDataResult<IEnumerable<Ders>>>
    {
        public class GetDersesQueryHandler : IRequestHandler<GetDersesQuery, IDataResult<IEnumerable<Ders>>>
        {
            private readonly IDersRepository _dersRepository;
            private readonly IMediator _mediator;

            public GetDersesQueryHandler(IDersRepository dersRepository, IMediator mediator)
            {
                _dersRepository = dersRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<Ders>>> Handle(GetDersesQuery request, CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<Ders>>(await _dersRepository.GetListAsync());
            }
        }
    }
}