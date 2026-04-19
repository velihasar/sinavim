using Business.BusinessAspects;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.Project.DenemeSinaviDtos;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.DenemeSinavis.Queries
{
    /// <summary>
    /// Oturumdaki kullanıcının belirli bir sınav için denemeleri; yeniden eskiye, toplam net ile.
    /// </summary>
    public class GetMyDenemeSinavisForSinavQuery : IRequest<IDataResult<DenemeSinaviFormeSinavQueryResult>>
    {
        public int SinavId { get; set; }

        /// <summary>
        /// true: skip/take ile sayfalama; TotalCount döner. false: mevcut davranış (son N veya tümü).
        /// </summary>
        public bool Paged { get; set; }

        /// <summary>
        /// Sayfalı modda atlama (varsayılan 0).
        /// </summary>
        public int? Skip { get; set; }

        /// <summary>
        /// Kayıt limiti. Sayfalı modda sayfa boyutu (varsayılan 5). Sayfasız modda: son N veya tümü.
        /// </summary>
        public int? Take { get; set; }

        /// <summary>
        /// true ise her deneme için ders bazlı sonuçlar (D/Y/B, net) döner.
        /// </summary>
        public bool IncludeSonuclar { get; set; }

        /// <summary>
        /// İstenirse yalnız bu sınava bölümüne ait denemeler (sayfalı liste + filtre).
        /// </summary>
        public int? SinavBolumId { get; set; }

        public class GetMyDenemeSinavisForSinavQueryHandler
            : IRequestHandler<GetMyDenemeSinavisForSinavQuery, IDataResult<DenemeSinaviFormeSinavQueryResult>>
        {
            private readonly IDenemeSinaviRepository _denemeSinaviRepository;
            private readonly IDersRepository _dersRepository;

            public GetMyDenemeSinavisForSinavQueryHandler(
                IDenemeSinaviRepository denemeSinaviRepository,
                IDersRepository dersRepository)
            {
                _denemeSinaviRepository = denemeSinaviRepository;
                _dersRepository = dersRepository;
            }

            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<DenemeSinaviFormeSinavQueryResult>> Handle(
                GetMyDenemeSinavisForSinavQuery request,
                CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserId();
                if (userId == 0)
                {
                    return new ErrorDataResult<DenemeSinaviFormeSinavQueryResult>("Oturum bulunamadı.");
                }

                if (request.SinavId <= 0)
                {
                    return new ErrorDataResult<DenemeSinaviFormeSinavQueryResult>("Sınav seçimi gerekli.");
                }

                var baseFilter = _denemeSinaviRepository.Query()
                    .AsNoTracking()
                    .Where(d => d.UserId == userId && d.SinavId == request.SinavId);

                if (request.SinavBolumId.HasValue && request.SinavBolumId.Value > 0)
                {
                    var bId = request.SinavBolumId.Value;
                    baseFilter = baseFilter.Where(d => d.SinavBolumId == bId);
                }

                var totalCount = 0;
                if (request.Paged)
                {
                    totalCount = await baseFilter.CountAsync(cancellationToken);
                }

                IQueryable<DenemeSinavi> withIncludes;
                if (request.IncludeSonuclar)
                {
                    withIncludes = baseFilter
                        .Include(d => d.SinavBolum)
                        .Include(d => d.Sonuclar);
                }
                else
                {
                    withIncludes = baseFilter
                        .Include(d => d.SinavBolum)
                        .Include(d => d.Sonuclar);
                }

                var ordered = withIncludes
                    .OrderByDescending(d => d.Tarih)
                    .ThenByDescending(d => d.Id);

                List<DenemeSinavi> list;
                if (request.Paged)
                {
                    var skip = request.Skip.HasValue && request.Skip.Value > 0 ? request.Skip.Value : 0;
                    var take = request.Take is > 0 ? request.Take!.Value : 5;
                    list = await ordered.Skip(skip).Take(take).ToListAsync(cancellationToken);
                }
                else if (request.Take.HasValue && request.Take.Value > 0)
                {
                    list = await ordered.Take(request.Take.Value).ToListAsync(cancellationToken);
                }
                else
                {
                    list = await ordered.ToListAsync(cancellationToken);
                }

                Dictionary<(int SinavId, int? SinavBolumId), List<Ders>> dersTemplateCache = null;
                if (request.IncludeSonuclar && list.Count > 0)
                {
                    dersTemplateCache = new Dictionary<(int, int?), List<Ders>>();
                    var keys = list
                        .Select(x => (x.SinavId, x.SinavBolumId))
                        .Distinct()
                        .ToList();
                    foreach (var key in keys)
                    {
                        var q = _dersRepository.Query()
                            .AsNoTracking()
                            .Where(x => x.SinavId == key.SinavId);
                        if (key.SinavBolumId.HasValue && key.SinavBolumId.Value > 0)
                        {
                            var bid = key.SinavBolumId.Value;
                            q = q.Where(x => x.SinavBolumId == bid);
                        }

                        var rows = await q
                            .OrderBy(x => x.SiraNo)
                            .ThenBy(x => x.Id)
                            .ToListAsync(cancellationToken);
                        dersTemplateCache[key] = rows;
                    }
                }

                var dtoList = list.Select(d => new DenemeSinaviOzetListDto
                {
                    Id = d.Id,
                    Ad = d.Ad,
                    SinavId = d.SinavId,
                    SinavBolumId = d.SinavBolumId,
                    SinavBolumIsim = d.SinavBolum != null ? d.SinavBolum.Isim : null,
                    Tarih = d.Tarih,
                    ToplamNet = d.Sonuclar != null && d.Sonuclar.Count > 0
                        ? d.Sonuclar.Sum(s => s.ToplamNet)
                        : 0m,
                    Sonuclar = request.IncludeSonuclar
                        ? DenemeSinaviSonucMapper.BuildSonuclarFullDers(d, dersTemplateCache)
                        : null,
                }).ToList();

                var result = new DenemeSinaviFormeSinavQueryResult
                {
                    Items = dtoList,
                    TotalCount = request.Paged ? totalCount : 0,
                };

                return new SuccessDataResult<DenemeSinaviFormeSinavQueryResult>(result);
            }
        }
    }
}
