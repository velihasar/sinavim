using System;

namespace Core.Entities.Dtos.Project.SinavDtos
{
    public class CreateSinavDto : IDto
    {
        public int Id { get; set; }
        public string KısaAd { get; set; }
        public string Ad { get; set; }
        public DateTime Tarih { get; set; }
        public int DogruyuGoturenYanlisSay { get; set; }
    }
}
