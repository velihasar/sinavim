using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.Project.DenemeSinaviDtos;
using System.Collections.Generic;
using System.Linq;

namespace Business.Handlers.DenemeSinavis
{
    internal static class DenemeSinaviSonucMapper
    {
        /// <summary>
        /// Sınava (ve denemede seçiliyse bölüme) bağlı tüm dersleri sırayla döner;
        /// sonuç girilmemiş dersler 0 D/Y/B ve 0 net ile gelir.
        /// Kayıtlı sonuç için <see cref="DenemeSinavSonucOzetDto.Id"/> veritabanı Id'sidir; yoksa 0.
        /// </summary>
        public static List<DenemeSinavSonucOzetDto> BuildSonuclarFullDers(
            DenemeSinavi d,
            Dictionary<(int SinavId, int? SinavBolumId), List<Ders>> dersTemplateCache)
        {
            if (dersTemplateCache == null)
            {
                return new List<DenemeSinavSonucOzetDto>();
            }

            var key = (d.SinavId, d.SinavBolumId);
            if (!dersTemplateCache.TryGetValue(key, out var template) || template == null || template.Count == 0)
            {
                if (d.Sonuclar == null || d.Sonuclar.Count == 0)
                {
                    return new List<DenemeSinavSonucOzetDto>();
                }

                return d.Sonuclar
                    .OrderBy(s => s.DersId)
                    .Select(s => new DenemeSinavSonucOzetDto
                    {
                        Id = s.Id,
                        DersId = s.DersId,
                        DersAd = string.Empty,
                        DogruSayisi = s.DogruSayisi,
                        YanlisSayisi = s.YanlisSayisi,
                        BosSayisi = s.BosSayisi,
                        ToplamNet = s.ToplamNet,
                    })
                    .ToList();
            }

            var byDersId = (d.Sonuclar ?? Enumerable.Empty<DenemeSinavSonucu>())
                .GroupBy(s => s.DersId)
                .ToDictionary(g => g.Key, g => g.First());

            var list = new List<DenemeSinavSonucOzetDto>(template.Count);
            foreach (var der in template)
            {
                if (byDersId.TryGetValue(der.Id, out var s))
                {
                    list.Add(new DenemeSinavSonucOzetDto
                    {
                        Id = s.Id,
                        DersId = der.Id,
                        DersAd = der.Ad ?? string.Empty,
                        DogruSayisi = s.DogruSayisi,
                        YanlisSayisi = s.YanlisSayisi,
                        BosSayisi = s.BosSayisi,
                        ToplamNet = s.ToplamNet,
                    });
                }
                else
                {
                    list.Add(new DenemeSinavSonucOzetDto
                    {
                        Id = 0,
                        DersId = der.Id,
                        DersAd = der.Ad ?? string.Empty,
                        DogruSayisi = 0,
                        YanlisSayisi = 0,
                        BosSayisi = 0,
                        ToplamNet = 0m,
                    });
                }
            }

            return list;
        }
    }
}
