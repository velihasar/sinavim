using System;

namespace Core.Entities.Dtos.Project.SinavDtos
{
    public class SinavDto : IDto
    {
        public int Id { get; set; }
        public string KısaAd { get; set; }
        public string Ad { get; set; }
        public string? Aciklama { get; set; }
        public DateTime Tarih { get; set; }
        public int SiraNo { get; set; }
        public int DogruyuGoturenYanlisSay { get; set; }
    }
}
