
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
using System.Linq;
using Core.Entities.Dtos.Project.KullaniciSinavDtos;

namespace Business.Handlers.KullaniciSinavs.Queries
{

    public class GetKullaniciSinavsQuery : IRequest<IDataResult<IEnumerable<KullaniciSinavListDto>>>
    {
        public class GetKullaniciSinavsQueryHandler : IRequestHandler<GetKullaniciSinavsQuery, IDataResult<IEnumerable<KullaniciSinavListDto>>>
        {
            private readonly IKullaniciSinavRepository _kullaniciSinavRepository;
            private readonly IMediator _mediator;

            public GetKullaniciSinavsQueryHandler(IKullaniciSinavRepository kullaniciSinavRepository, IMediator mediator)
            {
                _kullaniciSinavRepository = kullaniciSinavRepository;
                _mediator = mediator;
            }

            //[PerformanceAspect(5)]
            //[CacheAspect(10)]
            //[LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<KullaniciSinavListDto>>> Handle(GetKullaniciSinavsQuery request, CancellationToken cancellationToken)
            {
                var list = await _kullaniciSinavRepository.GetListAsync();
                var dtoList = list.Select(k => new KullaniciSinavListDto
                {
                    Id = k.Id,
                    UserId = k.UserId,
                    SinavId = k.SinavId,
                    HedefPuan = k.HedefPuan
                });

                return new SuccessDataResult<IEnumerable<KullaniciSinavListDto>>(dtoList);
            }
        }
    }
}