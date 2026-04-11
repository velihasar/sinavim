using System;

namespace Core.Entities.Dtos.Project.DersDtos
{
    public class CreateDersDto : IDto
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public string IkonAnahtari { get; set; }
        public int SiraNo { get; set; }
        public int SinavId { get; set; }
        public int? SinavBolumId { get; set; }
    }
}
