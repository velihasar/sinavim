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
    /// Oturumdaki kullanıcı için günlük çözülen soru listesi (bugünden geçmişe); eksik günler 0.
    /// En fazla son 90 takvim günü (bugün dahil, Türkiye saati); daha eski günler dönülmez.
    /// </summary>
    public class GetGunlukSoruCozumuPageForMeQuery : IRequest<IDataResult<GunlukSoruCozumuPageForMeDto>>
    {
        /// <summary>Bugün dahil listelenecek maksimum gün sayısı.</summary>
        public const int MaxGunlukPenceresi = 90;

        public int OffsetDays { get; set; }
        public int Take { get; set; } = 30;

        public class GetGunlukSoruCozumuPageForMeQueryHandler
            : IRequestHandler<GetGunlukSoruCozumuPageForMeQuery, IDataResult<GunlukSoruCozumuPageForMeDto>>
        {
            private readonly IKullaniciGunlukSoruCozumuRepository _repo;

            public GetGunlukSoruCozumuPageForMeQueryHandler(IKullaniciGunlukSoruCozumuRepository repo)
            {
                _repo = repo;
            }

            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<GunlukSoruCozumuPageForMeDto>> Handle(
                GetGunlukSoruCozumuPageForMeQuery request,
                CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserId();
                if (userId == 0)
                {
                    return new ErrorDataResult<GunlukSoruCozumuPageForMeDto>("Oturum bulunamadı.");
                }

                var take = Math.Clamp(request.Take, 1, 60);
                var offset = Math.Max(0, request.OffsetDays);
                var today = DateTimeExtensions.TurkeyTodayToNpgsqlDateOnly();
                var oldestAllowed = today.AddDays(-(MaxGunlukPenceresi - 1));
                var batchNewest = today.AddDays(-offset);

                if (batchNewest < oldestAllowed)
                {
                    return new SuccessDataResult<GunlukSoruCozumuPageForMeDto>(new GunlukSoruCozumuPageForMeDto
                    {
                        Items = new List<GunlukSoruGunOzetDto>(),
                        NextOffsetDays = offset,
                        HasMore = false,
                    });
                }

                var dayList = new List<DateTime>(take);
                for (var i = 0; i < take; i++)
                {
                    var d = batchNewest.AddDays(-i);
                    if (d < oldestAllowed)
                    {
                        break;
                    }

                    dayList.Add(d);
                }

                if (dayList.Count == 0)
                {
                    return new SuccessDataResult<GunlukSoruCozumuPageForMeDto>(new GunlukSoruCozumuPageForMeDto
                    {
                        Items = new List<GunlukSoruGunOzetDto>(),
                        NextOffsetDays = offset,
                        HasMore = false,
                    });
                }

                var oldest = dayList[dayList.Count - 1];
                var newest = dayList[0];
                var rangeEndExclusive = newest.AddDays(1);

                var rows = await _repo.Query()
                    .Where(x => x.UserId == userId && x.Tarih >= oldest && x.Tarih < rangeEndExclusive)
                    .ToListAsync(cancellationToken);

                var sums = rows
                    .GroupBy(x => x.Tarih.ToNpgsqlDateOnly())
                    .ToDictionary(g => g.Key, g => g.Sum(a => a.CozulenSoruSayisi));

                var items = new List<GunlukSoruGunOzetDto>(dayList.Count);
                foreach (var d in dayList)
                {
                    items.Add(new GunlukSoruGunOzetDto
                    {
                        Tarih = d,
                        CozulenSoruSayisi = sums.TryGetValue(d, out var c) ? c : 0,
                    });
                }

                var hasMore = dayList.Count == take && oldest > oldestAllowed;
                var nextOffset = offset + dayList.Count;

                return new SuccessDataResult<GunlukSoruCozumuPageForMeDto>(new GunlukSoruCozumuPageForMeDto
                {
                    Items = items,
                    NextOffsetDays = nextOffset,
                    HasMore = hasMore,
                });
            }
        }
    }
}
