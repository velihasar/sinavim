using System;
using System.Collections.Generic;

namespace Core.Entities.Dtos.Project.DenemeSinaviDtos
{
    /// <summary>
    /// Son denemeler listesi: kullanıcı + sınav bazlı özet (toplam net dahil).
    /// </summary>
    public class DenemeSinaviOzetListDto : IDto
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public int SinavId { get; set; }
        public int? SinavBolumId { get; set; }
        public string SinavBolumIsim { get; set; }
        public DateTime Tarih { get; set; }
        public decimal ToplamNet { get; set; }

        /// <summary>
        /// İstenirse (includeSonuclar) ders bazlı sonuçlar; aksi halde null.
        /// </summary>
        public List<DenemeSinavSonucOzetDto> Sonuclar { get; set; }
    }
}
