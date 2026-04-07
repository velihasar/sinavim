
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

namespace Business.Handlers.KullaniciSinavs.Queries
{

    public class GetKullaniciSinavsQuery : IRequest<IDataResult<IEnumerable<KullaniciSinav>>>
    {
        public class GetKullaniciSinavsQueryHandler : IRequestHandler<GetKullaniciSinavsQuery, IDataResult<IEnumerable<KullaniciSinav>>>
        {
            private readonly IKullaniciSinavRepository _kullaniciSinavRepository;
            private readonly IMediator _mediator;

            public GetKullaniciSinavsQueryHandler(IKullaniciSinavRepository kullaniciSinavRepository, IMediator mediator)
            {
                _kullaniciSinavRepository = kullaniciSinavRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<KullaniciSinav>>> Handle(GetKullaniciSinavsQuery request, CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<KullaniciSinav>>(await _kullaniciSinavRepository.GetListAsync());
            }
        }
    }
}