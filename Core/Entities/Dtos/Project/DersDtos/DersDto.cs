using System;

namespace Core.Entities.Dtos.Project.DersDtos
{
    public class DersDto : IDto
    {
        public int Id { get; set; }
        public string Ad { get; set; }

        /// <summary>Ionicons adı (ör. calculator-outline).</summary>
        public string IkonAnahtari { get; set; }

        public int SiraNo { get; set; }

        public int SinavId { get; set; }
        public int? SinavBolumId { get; set; }
        public string SinavBolumIsim { get; set; }
    }
}
