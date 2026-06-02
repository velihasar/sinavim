using Core.Entities.Concrete;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.Project.ArkadaslikDtos;
using Core.Enums;
using Core.Extensions;
using DataAccess.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Helpers
{
    internal sealed class ArkadasRekabetParticipant
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public bool BenMi { get; set; }
        public int? SinavId { get; set; }
    }

    internal static class ArkadasRekabetHelper
    {
        public static async Task<List<ArkadasRekabetParticipant>> LoadParticipantsAsync(
            IArkadaslikRepository arkadaslikRepository,
            IUserRepository userRepository,
            IKullaniciSinavRepository kullaniciSinavRepository,
            int userId,
            CancellationToken cancellationToken)
        {
            var me = await userRepository.GetAsync(u => u.UserId == userId);
            var meName = me?.FullName?.Trim();
            if (string.IsNullOrEmpty(meName))
            {
                meName = "Sen";
            }

            var friendships = await arkadaslikRepository.Query()
                .Include(x => x.UserKucuk)
                .Include(x => x.UserBuyuk)
                .Where(x => (x.UserIdKucuk == userId || x.UserIdBuyuk == userId)
                            && x.IsActive != false)
                .ToListAsync(cancellationToken);

            var participants = new List<ArkadasRekabetParticipant>
            {
                new()
                {
                    UserId = userId,
                    FullName = meName,
                    BenMi = true,
                },
            };

            foreach (var x in friendships)
            {
                var arkadas = x.UserIdKucuk == userId ? x.UserBuyuk : x.UserKucuk;
                var arkadasUserId = x.UserIdKucuk == userId ? x.UserIdBuyuk : x.UserIdKucuk;
                participants.Add(new ArkadasRekabetParticipant
                {
                    UserId = arkadasUserId,
                    FullName = string.IsNullOrWhiteSpace(arkadas?.FullName)
                        ? "Kullanıcı"
                        : arkadas.FullName.Trim(),
                    BenMi = false,
                });
            }

            var userIds = participants.Select(p => p.UserId).Distinct().ToList();
            var sinavRows = await kullaniciSinavRepository.Query()
                .Where(k => userIds.Contains(k.UserId))
                .ToListAsync(cancellationToken);
            var sinavByUser = sinavRows
                .GroupBy(k => k.UserId)
                .ToDictionary(g => g.Key, g => g.First().SinavId);

            foreach (var p in participants)
            {
                if (sinavByUser.TryGetValue(p.UserId, out var sid))
                {
                    p.SinavId = sid;
                }
            }

            return participants;
        }

        public static async Task<ArkadasRekabetDto> BuildRekabetAsync(
            IArkadaslikRepository arkadaslikRepository,
            IUserRepository userRepository,
            IKullaniciSinavRepository kullaniciSinavRepository,
            IKullaniciGunlukSoruCozumuRepository gunlukSoruRepository,
            IDenemeSinaviRepository denemeSinaviRepository,
            IKullaniciKonuIlerlemeRepository konuIlerlemeRepository,
            IKonuRepository konuRepository,
            IDersRepository dersRepository,
            ISinavRepository sinavRepository,
            int userId,
            int gunSayisi,
            CancellationToken cancellationToken)
        {
            var participants = await LoadParticipantsAsync(
                arkadaslikRepository,
                userRepository,
                kullaniciSinavRepository,
                userId,
                cancellationToken);

            var arkadasSayisi = participants.Count(p => !p.BenMi);
            var today = DateTimeExtensions.TurkeyTodayToNpgsqlDateOnly();
            var gun = Math.Clamp(gunSayisi, 7, 30);
            var start = today.AddDays(-(gun - 1));
            var endExclusive = today.AddDays(1);
            var periodStartTs = start.ToNpgsqlTimestamp();

            var dto = new ArkadasRekabetDto
            {
                HasArkadas = arkadasSayisi > 0,
                ArkadasSayisi = arkadasSayisi,
                Donem = new ArkadasRekabetDonemDto
                {
                    Baslangic = start,
                    Bitis = today,
                    GunSayisi = gun,
                },
            };

            if (arkadasSayisi == 0)
            {
                return dto;
            }

            var userIds = participants.Select(p => p.UserId).ToList();
            var me = participants.First(p => p.BenMi);
            var mySinavId = me.SinavId;

            if (mySinavId.HasValue && mySinavId.Value > 0)
            {
                dto.SinavId = mySinavId.Value;
                var sinav = await sinavRepository.GetAsync(s => s.Id == mySinavId.Value);
                dto.SinavKisaAd = sinav?.KisaAd?.Trim() ?? sinav?.Ad?.Trim();
            }

            // --- Soru ---
            var soruRows = await gunlukSoruRepository.Query()
                .Where(x => userIds.Contains(x.UserId)
                            && x.Tarih >= start
                            && x.Tarih < endExclusive)
                .ToListAsync(cancellationToken);

            var soruByUserDay = soruRows
                .GroupBy(x => (x.UserId, Day: x.Tarih.ToNpgsqlDateOnly()))
                .ToDictionary(g => g.Key, g => g.Sum(a => a.CozulenSoruSayisi));

            var soruStats = new List<(ArkadasRekabetParticipant P, int Bugun, int Toplam, int GunBirincilik)>();
            foreach (var p in participants)
            {
                var bugun = soruByUserDay.TryGetValue((p.UserId, today), out var b) ? b : 0;
                var toplam = 0;
                for (var i = 0; i < gun; i++)
                {
                    var d = start.AddDays(i);
                    if (soruByUserDay.TryGetValue((p.UserId, d), out var c))
                    {
                        toplam += c;
                    }
                }

                var gunBirincilik = 0;
                for (var i = 0; i < gun; i++)
                {
                    var d = start.AddDays(i);
                    var dayCounts = participants
                        .Select(pp => soruByUserDay.TryGetValue((pp.UserId, d), out var c) ? c : 0)
                        .ToList();
                    var max = dayCounts.Max();
                    if (max <= 0)
                    {
                        continue;
                    }

                    var myDay = soruByUserDay.TryGetValue((p.UserId, d), out var mc) ? mc : 0;
                    if (myDay == max)
                    {
                        gunBirincilik++;
                    }
                }

                soruStats.Add((p, bugun, toplam, gunBirincilik));
            }

            var soruOrdered = soruStats
                .OrderByDescending(x => x.Toplam)
                .ThenByDescending(x => x.GunBirincilik)
                .ThenBy(x => x.P.FullName, StringComparer.Create(new System.Globalization.CultureInfo("tr-TR"), false))
                .ToList();

            dto.SoruSiralama = soruOrdered.Select((x, idx) => new ArkadasRekabetSoruSatirDto
            {
                UserId = x.P.UserId,
                FullName = x.P.FullName,
                BenMi = x.P.BenMi,
                BugunCozulenSoru = x.Bugun,
                DonemToplamSoru = x.Toplam,
                GunBirincilikSayisi = x.GunBirincilik,
                Sira = idx + 1,
            }).ToList();

            if (!mySinavId.HasValue || mySinavId.Value <= 0)
            {
                return dto;
            }

            var sid = mySinavId.Value;

            // --- Deneme ---
            var denemeler = await denemeSinaviRepository.Query()
                .Include(d => d.Sonuclar)
                .Where(d => userIds.Contains(d.UserId)
                            && d.SinavId == sid
                            && d.Tarih >= start
                            && d.Tarih < endExclusive)
                .ToListAsync(cancellationToken);

            var denemeByUserDay = denemeler
                .GroupBy(d => (d.UserId, Day: d.Tarih.ToNpgsqlDateOnly()))
                .ToDictionary(
                    g => g.Key,
                    g => g.Max(d => d.Sonuclar != null && d.Sonuclar.Count > 0
                        ? d.Sonuclar.Sum(s => s.ToplamNet)
                        : 0m));

            var denemeStats = new List<(ArkadasRekabetParticipant P, bool Ok, decimal EnIyi, int Sayi, int OrtakGun, int OrtakBirincilik)>();
            foreach (var p in participants)
            {
                var comparable = p.SinavId == sid;
                if (!comparable)
                {
                    denemeStats.Add((p, false, 0m, 0, 0, 0));
                    continue;
                }

                var userDenemeler = denemeler.Where(d => d.UserId == p.UserId).ToList();
                var enIyi = userDenemeler.Count == 0
                    ? 0m
                    : userDenemeler.Max(d => d.Sonuclar != null && d.Sonuclar.Count > 0
                        ? d.Sonuclar.Sum(s => s.ToplamNet)
                        : 0m);

                var ortakGun = 0;
                var ortakBirincilik = 0;
                var comparableIds = participants.Where(pp => pp.SinavId == sid).Select(pp => pp.UserId).ToHashSet();

                for (var i = 0; i < gun; i++)
                {
                    var d = start.AddDays(i);
                    var whoHas = comparableIds
                        .Where(uid => denemeByUserDay.ContainsKey((uid, d)))
                        .ToList();
                    if (whoHas.Count < 2 || !whoHas.Contains(p.UserId))
                    {
                        continue;
                    }

                    ortakGun++;
                    var dayNets = whoHas
                        .Select(uid => denemeByUserDay.TryGetValue((uid, d), out var n) ? n : 0m)
                        .ToList();
                    var maxNet = dayNets.Max();
                    var myNet = denemeByUserDay.TryGetValue((p.UserId, d), out var mn) ? mn : 0m;
                    if (myNet == maxNet)
                    {
                        ortakBirincilik++;
                    }
                }

                denemeStats.Add((p, true, enIyi, userDenemeler.Count, ortakGun, ortakBirincilik));
            }

            var denemeOrdered = denemeStats
                .Where(x => x.Ok)
                .OrderByDescending(x => x.EnIyi)
                .ThenByDescending(x => x.OrtakBirincilik)
                .ThenByDescending(x => x.Sayi)
                .ToList();

            dto.DenemeSiralama = denemeOrdered.Select((x, idx) => new ArkadasRekabetDenemeSatirDto
            {
                UserId = x.P.UserId,
                FullName = x.P.FullName,
                BenMi = x.P.BenMi,
                Karsilastirilabilir = true,
                EnIyiNet = x.EnIyi,
                DenemeSayisi = x.Sayi,
                OrtakGunSayisi = x.OrtakGun,
                OrtakGundeBirincilikSayisi = x.OrtakBirincilik,
                Sira = idx + 1,
            }).ToList();

            foreach (var x in denemeStats.Where(x => !x.Ok))
            {
                dto.DenemeSiralama.Add(new ArkadasRekabetDenemeSatirDto
                {
                    UserId = x.P.UserId,
                    FullName = x.P.FullName,
                    BenMi = x.P.BenMi,
                    Karsilastirilabilir = false,
                    Sira = 0,
                });
            }

            // --- Konu ---
            var dersIds = await dersRepository.Query()
                .Where(d => d.SinavId == sid && (d.IsActive != false))
                .Select(d => d.Id)
                .ToListAsync(cancellationToken);

            var konuIds = dersIds.Count == 0
                ? new List<int>()
                : await konuRepository.Query()
                    .Where(k => dersIds.Contains(k.DersId) && (k.IsActive != false))
                    .Select(k => k.Id)
                    .ToListAsync(cancellationToken);

            var toplamKonu = konuIds.Count;
            var ilerlemeler = konuIds.Count == 0
                ? new List<KullaniciKonuIlerleme>()
                : await konuIlerlemeRepository.Query()
                    .Where(p => userIds.Contains(p.UserId) && konuIds.Contains(p.KonuId))
                    .ToListAsync(cancellationToken);

            var konuStats = new List<(ArkadasRekabetParticipant P, bool Ok, int Tamamlanan, int Donemde)>();
            foreach (var p in participants)
            {
                if (p.SinavId != sid || toplamKonu == 0)
                {
                    konuStats.Add((p, false, 0, 0));
                    continue;
                }

                var userIl = ilerlemeler.Where(i => i.UserId == p.UserId).ToList();
                var tamamlanan = userIl.Count(i => i.Durum == IlerlemeDurumu.Tamamladi);
                var donemde = userIl.Count(i =>
                    i.Durum == IlerlemeDurumu.Tamamladi
                    && i.UpdatedDate.HasValue
                    && i.UpdatedDate.Value >= periodStartTs);

                konuStats.Add((p, true, tamamlanan, donemde));
            }

            var konuOrdered = konuStats
                .Where(x => x.Ok)
                .OrderByDescending(x => x.Donemde)
                .ThenByDescending(x => x.Tamamlanan)
                .ToList();

            dto.KonuSiralama = konuOrdered.Select((x, idx) => new ArkadasRekabetKonuSatirDto
            {
                UserId = x.P.UserId,
                FullName = x.P.FullName,
                BenMi = x.P.BenMi,
                Karsilastirilabilir = true,
                TamamlananKonu = x.Tamamlanan,
                ToplamKonu = toplamKonu,
                Yuzde = toplamKonu > 0 ? (int)Math.Round(x.Tamamlanan * 100.0 / toplamKonu) : 0,
                DonemdeTamamlanan = x.Donemde,
                Sira = idx + 1,
            }).ToList();

            foreach (var x in konuStats.Where(x => !x.Ok))
            {
                dto.KonuSiralama.Add(new ArkadasRekabetKonuSatirDto
                {
                    UserId = x.P.UserId,
                    FullName = x.P.FullName,
                    BenMi = x.P.BenMi,
                    Karsilastirilabilir = false,
                    Sira = 0,
                });
            }

            return dto;
        }

        public static ArkadasRekabetDashboardOzetDto BuildDashboardOzet(
            ArkadasRekabetDto rekabet,
            int userId)
        {
            var ozet = new ArkadasRekabetDashboardOzetDto
            {
                HasArkadas = rekabet.HasArkadas,
                ArkadasSayisi = rekabet.ArkadasSayisi,
            };

            if (!rekabet.HasArkadas)
            {
                ozet.OzetMetin = string.Empty;
                return ozet;
            }

            var benSoru = rekabet.SoruSiralama.FirstOrDefault(x => x.BenMi);
            if (benSoru == null)
            {
                ozet.OzetMetin = string.Empty;
                return ozet;
            }

            ozet.BugunBenimSoru = benSoru.BugunCozulenSoru;
            ozet.BugunSira = benSoru.Sira;
            ozet.HaftaToplamSoru = benSoru.DonemToplamSoru;
            ozet.HaftaSira = benSoru.Sira;
            ozet.ToplamKisi = rekabet.SoruSiralama.Count;

            var birinci = rekabet.SoruSiralama.FirstOrDefault(x => x.Sira == 1);
            if (birinci != null && !birinci.BenMi)
            {
                ozet.BirinciFullName = birinci.FullName;
                ozet.BirincidenFark = birinci.BugunCozulenSoru - benSoru.BugunCozulenSoru;
            }

            if (benSoru.Sira == 1)
            {
                ozet.OzetMetin = ozet.BugunBenimSoru > 0
                    ? $"Bugün {ozet.BugunBenimSoru} soru ile 1. sıradasın."
                    : "Bugün henüz soru girmedin; arkadaşların arasında 1. sıradasın.";
            }
            else if (birinci != null && ozet.BirincidenFark.HasValue && ozet.BirincidenFark.Value > 0)
            {
                ozet.OzetMetin =
                    $"Bugün {ozet.BugunBenimSoru} soru · {ozet.BugunSira}. sıra · {birinci.FullName}'ten {ozet.BirincidenFark.Value} geridesin.";
            }
            else
            {
                ozet.OzetMetin =
                    $"Bugün {ozet.BugunBenimSoru} soru · arkadaşların arasında {ozet.BugunSira}. sıradasın.";
            }

            return ozet;
        }
    }
}
