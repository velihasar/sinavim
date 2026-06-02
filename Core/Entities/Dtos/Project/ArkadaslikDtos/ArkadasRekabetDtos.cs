using System;
using System.Collections.Generic;
using Core.Entities;

namespace Core.Entities.Dtos.Project.ArkadaslikDtos
{
    public class ArkadasRekabetDonemDto : IDto
    {
        public DateTime Baslangic { get; set; }
        public DateTime Bitis { get; set; }
        public int GunSayisi { get; set; }
    }

    public class ArkadasRekabetSoruSatirDto : IDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public bool BenMi { get; set; }
        public int BugunCozulenSoru { get; set; }
        public int DonemToplamSoru { get; set; }
        public int GunBirincilikSayisi { get; set; }
        public int Sira { get; set; }
    }

    public class ArkadasRekabetDenemeSatirDto : IDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public bool BenMi { get; set; }
        public bool Karsilastirilabilir { get; set; }
        public decimal EnIyiNet { get; set; }
        public int DenemeSayisi { get; set; }
        public int OrtakGunSayisi { get; set; }
        public int OrtakGundeBirincilikSayisi { get; set; }
        public int Sira { get; set; }
    }

    public class ArkadasRekabetKonuSatirDto : IDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public bool BenMi { get; set; }
        public bool Karsilastirilabilir { get; set; }
        public int TamamlananKonu { get; set; }
        public int ToplamKonu { get; set; }
        public int Yuzde { get; set; }
        public int DonemdeTamamlanan { get; set; }
        public int Sira { get; set; }
    }

    public class ArkadasRekabetDto : IDto
    {
        public bool HasArkadas { get; set; }
        public int ArkadasSayisi { get; set; }
        public ArkadasRekabetDonemDto Donem { get; set; }
        public int? SinavId { get; set; }
        public string SinavKisaAd { get; set; }
        public List<ArkadasRekabetSoruSatirDto> SoruSiralama { get; set; } = new();
        public List<ArkadasRekabetDenemeSatirDto> DenemeSiralama { get; set; } = new();
        public List<ArkadasRekabetKonuSatirDto> KonuSiralama { get; set; } = new();
    }

    public class ArkadasRekabetDashboardOzetDto : IDto
    {
        public bool HasArkadas { get; set; }
        public int ArkadasSayisi { get; set; }
        public int BugunBenimSoru { get; set; }
        public int BugunSira { get; set; }
        public int ToplamKisi { get; set; }
        public string BirinciFullName { get; set; }
        public int? BirincidenFark { get; set; }
        public int HaftaToplamSoru { get; set; }
        public int HaftaSira { get; set; }
        public string OzetMetin { get; set; }
    }

}
