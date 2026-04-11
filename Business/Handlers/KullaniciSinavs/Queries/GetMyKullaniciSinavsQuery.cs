
using Business.BusinessAspects;
using Core.Entities.Dtos.Project.KullaniciSinavDtos;
using Core.Extensions;
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
    /// <summary>
    /// Oturumdaki kullanıcının KullaniciSinav kayıtları (sınav seçimi var mı kontrolü için).
    /// </summary>
    public class GetMyKullaniciSinavsQuery : IRequest<IDataResult<IEnumerable<KullaniciSinavListDto>>>
    {
        public class GetMyKullaniciSinavsQueryHandler
            : IRequestHandler<GetMyKullaniciSinavsQuery, IDataResult<IEnumerable<KullaniciSinavListDto>>>
        {
            private readonly IKullaniciSinavRepository _kullaniciSinavRepository;

            public GetMyKullaniciSinavsQueryHandler(IKullaniciSinavRepository kullaniciSinavRepository)
            {
                _kullaniciSinavRepository = kullaniciSinavRepository;
            }

            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<KullaniciSinavListDto>>> Handle(
                GetMyKullaniciSinavsQuery request,
                CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserId();
                if (userId == 0)
                {
                    return new ErrorDataResult<IEnumerable<KullaniciSinavListDto>>("Oturum bulunamadı.");
                }

                var list = await _kullaniciSinavRepository.Query()
                    .Include(k => k.Sinav)
                    .Where(k => k.UserId == userId)
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
