using System;

namespace Core.Entities.Dtos.Project.KonuDtos
{
    public class CreateKonuDto : IDto
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public int SiraNo { get; set; }
        public int DersId { get; set; }
    }
}
