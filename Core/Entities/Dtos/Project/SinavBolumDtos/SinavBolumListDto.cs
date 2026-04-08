using System;

namespace Core.Entities.Dtos.Project.SinavBolumDtos
{
    public class SinavBolumListDto : IDto
    {
        public int Id { get; set; }
        public int SinavId { get; set; }
        public string Isim { get; set; }
        public string SinavAd { get; set; }
    }
}
