
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

namespace Business.Handlers.KullaniciGunlukSoruCozumus.Queries
{

    public class GetKullaniciGunlukSoruCozumusQuery : IRequest<IDataResult<IEnumerable<KullaniciGunlukSoruCozumu>>>
    {
        public class GetKullaniciGunlukSoruCozumusQueryHandler : IRequestHandler<GetKullaniciGunlukSoruCozumusQuery, IDataResult<IEnumerable<KullaniciGunlukSoruCozumu>>>
        {
            private readonly IKullaniciGunlukSoruCozumuRepository _kullaniciGunlukSoruCozumuRepository;
            private readonly IMediator _mediator;

            public GetKullaniciGunlukSoruCozumusQueryHandler(IKullaniciGunlukSoruCozumuRepository kullaniciGunlukSoruCozumuRepository, IMediator mediator)
            {
                _kullaniciGunlukSoruCozumuRepository = kullaniciGunlukSoruCozumuRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<KullaniciGunlukSoruCozumu>>> Handle(GetKullaniciGunlukSoruCozumusQuery request, CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<KullaniciGunlukSoruCozumu>>(await _kullaniciGunlukSoruCozumuRepository.GetListAsync());
            }
        }
    }
}