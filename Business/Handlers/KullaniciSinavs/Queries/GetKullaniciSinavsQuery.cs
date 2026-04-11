
using Business.BusinessAspects;
using Core.Entities.Dtos.Project.KullaniciSinavDtos;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
                var list = await _kullaniciSinavRepository.Query()
                    .Include(k => k.Sinav)
                    .ToListAsync(cancellationToken);

                var dtoList = list.Select(k => new KullaniciSinavListDto
                {
                    Id = k.Id,
                    UserId = k.UserId,
                    SinavId = k.SinavId,
                    HedefPuan = k.HedefPuan,
                    SinavTarih = k.Sinav?.Tarih,
                    SinavKisaAd = k.Sinav?.KısaAd,
                    SinavAd = k.Sinav?.Ad,
                });

                return new SuccessDataResult<IEnumerable<KullaniciSinavListDto>>(dtoList);
            }
        }
    }
}