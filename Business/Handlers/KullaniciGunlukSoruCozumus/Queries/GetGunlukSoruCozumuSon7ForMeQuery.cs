using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Core.Entities.Dtos.Project.KullaniciGunlukSoruCozumuDtos;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Handlers.KullaniciGunlukSoruCozumus.Queries
{
    /// <summary>
    /// Oturumdaki kullanıcı için anasayfa grafiği: görünür 7 gün + trend için bir önceki gün (Türkiye takvimi, toplam 8 gün); kayıt yoksa 0.
    /// </summary>
    public class GetGunlukSoruCozumuSon7ForMeQuery : IRequest<IDataResult<List<GunlukSoruGunOzetDto>>>
    {
        public class GetGunlukSoruCozumuSon7ForMeQueryHandler
            : IRequestHandler<GetGunlukSoruCozumuSon7ForMeQuery, IDataResult<List<GunlukSoruGunOzetDto>>>
        {
            private readonly IKullaniciGunlukSoruCozumuRepository _kullaniciGunlukSoruCozumuRepository;

            public GetGunlukSoruCozumuSon7ForMeQueryHandler(
                IKullaniciGunlukSoruCozumuRepository kullaniciGunlukSoruCozumuRepository)
            {
                _kullaniciGunlukSoruCozumuRepository = kullaniciGunlukSoruCozumuRepository;
            }

            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<List<GunlukSoruGunOzetDto>>> Handle(
                GetGunlukSoruCozumuSon7ForMeQuery request,
                CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserId();
                if (userId == 0)
                {
                    return new ErrorDataResult<List<GunlukSoruGunOzetDto>>("Oturum bulunamadı.");
                }

                var end = DateTimeExtensions.TurkeyTodayToNpgsqlDateOnly();
                var start = end.AddDays(-7);
                var rangeEndExclusive = end.AddDays(1);

                var rows = await _kullaniciGunlukSoruCozumuRepository.Query()
                    .Where(x => x.UserId == userId && x.Tarih >= start && x.Tarih < rangeEndExclusive)
                    .ToListAsync(cancellationToken);

                var sums = rows
                    .GroupBy(x => x.Tarih.ToNpgsqlDateOnly())
                    .ToDictionary(g => g.Key, g => g.Sum(a => a.CozulenSoruSayisi));

                var list = new List<GunlukSoruGunOzetDto>(8);
                for (var i = 0; i < 8; i++)
                {
                    var d = start.AddDays(i);
                    list.Add(new GunlukSoruGunOzetDto
                    {
                        Tarih = d,
                        CozulenSoruSayisi = sums.TryGetValue(d, out var c) ? c : 0,
                    });
                }

                return new SuccessDataResult<List<GunlukSoruGunOzetDto>>(list);
            }
        }
    }
}
