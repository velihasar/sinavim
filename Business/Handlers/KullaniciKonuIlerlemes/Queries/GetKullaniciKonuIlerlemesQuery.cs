
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

namespace Business.Handlers.KullaniciKonuIlerlemes.Queries
{

    public class GetKullaniciKonuIlerlemesQuery : IRequest<IDataResult<IEnumerable<KullaniciKonuIlerleme>>>
    {
        public class GetKullaniciKonuIlerlemesQueryHandler : IRequestHandler<GetKullaniciKonuIlerlemesQuery, IDataResult<IEnumerable<KullaniciKonuIlerleme>>>
        {
            private readonly IKullaniciKonuIlerlemeRepository _kullaniciKonuIlerlemeRepository;
            private readonly IMediator _mediator;

            public GetKullaniciKonuIlerlemesQueryHandler(IKullaniciKonuIlerlemeRepository kullaniciKonuIlerlemeRepository, IMediator mediator)
            {
                _kullaniciKonuIlerlemeRepository = kullaniciKonuIlerlemeRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<KullaniciKonuIlerleme>>> Handle(GetKullaniciKonuIlerlemesQuery request, CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<KullaniciKonuIlerleme>>(await _kullaniciKonuIlerlemeRepository.GetListAsync());
            }
        }
    }
}