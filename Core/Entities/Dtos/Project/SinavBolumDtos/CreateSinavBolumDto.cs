using System;

namespace Core.Entities.Dtos.Project.SinavBolumDtos
{
    public class CreateSinavBolumDto : IDto
    {
        public int Id { get; set; }
        public int SinavId { get; set; }
        public string Isim { get; set; }
    }
}
