using System.Collections.Generic;

namespace Core.Entities.Dtos.Project.KonuTakipDtos
{
    public class KonuTakipForMeDto : IDto
    {
        public int SinavId { get; set; }
        public string SinavKisaAd { get; set; }
        public string SinavAd { get; set; }

        /// <summary>Bölüm varken hangi bölüme göre dersler filtrelendi (UI seçimi).</summary>
        public int? AktifSinavBolumId { get; set; }

        public List<KonuTakipBolumOzetiDto> Bolumler { get; set; }
        public List<KonuTakipDersOzetDto> Dersler { get; set; }
    }
}
