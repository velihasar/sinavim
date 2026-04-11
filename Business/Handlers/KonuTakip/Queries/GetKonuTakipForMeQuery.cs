using Business.BusinessAspects;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.Project.KonuTakipDtos;
using Core.Enums;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.KonuTakip.Queries
{
    /// <summary>
    /// Oturumdaki kullanıcının seçtiği sınav için konu takip özeti.
    /// SinavBolumId verilirse yalnızca o bölüme bağlı dersler; verilmezse sınavdaki tüm dersler.
    /// </summary>
    public class GetKonuTakipForMeQuery : IRequest<IDataResult<KonuTakipForMeDto>>
    {
        public int? SinavBolumId { get; set; }

        public class GetKonuTakipForMeQueryHandler : IRequestHandler<GetKonuTakipForMeQuery, IDataResult<KonuTakipForMeDto>>
        {
            private readonly IKullaniciSinavRepository _kullaniciSinavRepository;
            private readonly ISinavRepository _sinavRepository;
            private readonly ISinavBolumRepository _sinavBolumRepository;
            private readonly IDersRepository _dersRepository;
            private readonly IKonuRepository _konuRepository;
            private readonly IKullaniciKonuIlerlemeRepository _ilerlemeRepository;

            public GetKonuTakipForMeQueryHandler(
                IKullaniciSinavRepository kullaniciSinavRepository,
                ISinavRepository sinavRepository,
                ISinavBolumRepository sinavBolumRepository,
                IDersRepository dersRepository,
                IKonuRepository konuRepository,
                IKullaniciKonuIlerlemeRepository ilerlemeRepository)
            {
                _kullaniciSinavRepository = kullaniciSinavRepository;
                _sinavRepository = sinavRepository;
                _sinavBolumRepository = sinavBolumRepository;
                _dersRepository = dersRepository;
                _konuRepository = konuRepository;
                _ilerlemeRepository = ilerlemeRepository;
            }

            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<KonuTakipForMeDto>> Handle(
                GetKonuTakipForMeQuery request,
                CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserId();
                if (userId == 0)
                {
                    return new ErrorDataResult<KonuTakipForMeDto>("Oturum bulunamadı.");
                }

                var ks = await _kullaniciSinavRepository.Query()
                    .FirstOrDefaultAsync(k => k.UserId == userId, cancellationToken);

                if (ks == null)
                {
                    return new ErrorDataResult<KonuTakipForMeDto>("Sınav seçimi bulunamadı.");
                }

                var sinav = await _sinavRepository.GetAsync(s => s.Id == ks.SinavId);
                if (sinav == null)
                {
                    return new ErrorDataResult<KonuTakipForMeDto>("Sınav bulunamadı.");
                }

                var bolumler = await _sinavBolumRepository.Query()
                    .Where(b => b.SinavId == sinav.Id && (b.IsActive != false))
                    .OrderBy(b => b.Id)
                    .Select(b => new KonuTakipBolumOzetiDto { Id = b.Id, Isim = b.Isim })
                    .ToListAsync(cancellationToken);

                var dersQuery = _dersRepository.Query().Where(d => d.SinavId == sinav.Id && (d.IsActive != false));

                int? aktifBolumId = null;
                if (bolumler.Count > 0)
                {
                    var bolumIdSet = bolumler.Select(b => b.Id).ToHashSet();
                    if (request.SinavBolumId.HasValue && bolumIdSet.Contains(request.SinavBolumId.Value))
                    {
                        aktifBolumId = request.SinavBolumId.Value;
                    }
                    else
                    {
                        aktifBolumId = bolumler[0].Id;
                    }

                    dersQuery = dersQuery.Where(d => d.SinavBolumId == aktifBolumId);
                }

                var dersler = await dersQuery.OrderBy(d => d.Id).ToListAsync(cancellationToken);
                if (dersler.Count == 0)
                {
                    return new SuccessDataResult<KonuTakipForMeDto>(new KonuTakipForMeDto
                    {
                        SinavId = sinav.Id,
                        SinavKisaAd = sinav.KısaAd,
                        SinavAd = sinav.Ad,
                        AktifSinavBolumId = aktifBolumId,
                        Bolumler = bolumler,
                        Dersler = new List<KonuTakipDersOzetDto>(),
                    });
                }

                var dersIds = dersler.Select(d => d.Id).ToList();
                var tumKonular = await _konuRepository.Query()
                    .Where(k => dersIds.Contains(k.DersId) && (k.IsActive != false))
                    .OrderBy(k => k.DersId)
                    .ThenBy(k => k.SiraNo)
                    .ToListAsync(cancellationToken);

                var konuIds = tumKonular.Select(k => k.Id).ToList();
                var ilerlemeler = await _ilerlemeRepository.Query()
                    .Where(p => p.UserId == userId && konuIds.Contains(p.KonuId))
                    .ToListAsync(cancellationToken);
                var ilerlemeByKonu = ilerlemeler.GroupBy(p => p.KonuId).ToDictionary(g => g.Key, g => g.First());

                var konuByDers = tumKonular.GroupBy(k => k.DersId).ToDictionary(g => g.Key, g => g.ToList());

                var dersOzetList = new List<KonuTakipDersOzetDto>();
                foreach (var ders in dersler)
                {
                    if (!konuByDers.TryGetValue(ders.Id, out var konular) || konular.Count == 0)
                    {
                        dersOzetList.Add(new KonuTakipDersOzetDto
                        {
                            Id = ders.Id,
                            Ad = ders.Ad,
                            ToplamKonu = 0,
                            TamamlananKonu = 0,
                            Konular = new List<KonuTakipKonuSatirDto>(),
                        });
                        continue;
                    }

                    var satirlar = new List<KonuTakipKonuSatirDto>();
                    var tam = 0;
                    foreach (var k in konular)
                    {
                        ilerlemeByKonu.TryGetValue(k.Id, out var il);
                        var durum = il?.Durum ?? IlerlemeDurumu.Baslamadi;
                        if (durum == IlerlemeDurumu.Tamamladi)
                        {
                            tam++;
                        }

                        satirlar.Add(new KonuTakipKonuSatirDto
                        {
                            Id = k.Id,
                            Ad = k.Ad,
                            SiraNo = k.SiraNo,
                            Durum = (short)durum,
                            IlerlemeKayitId = il?.Id,
                        });
                    }

                    dersOzetList.Add(new KonuTakipDersOzetDto
                    {
                        Id = ders.Id,
                        Ad = ders.Ad,
                        ToplamKonu = konular.Count,
                        TamamlananKonu = tam,
                        Konular = satirlar,
                    });
                }

                var dto = new KonuTakipForMeDto
                {
                    SinavId = sinav.Id,
                    SinavKisaAd = sinav.KısaAd,
                    SinavAd = sinav.Ad,
                    AktifSinavBolumId = aktifBolumId,
                    Bolumler = bolumler,
                    Dersler = dersOzetList,
                };

                return new SuccessDataResult<KonuTakipForMeDto>(dto);
            }
        }
    }
}
