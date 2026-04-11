using System.Collections.Generic;

namespace Core.Entities.Dtos.Project.KonuTakipDtos
{
    public class KonuTakipDersOzetDto : IDto
    {
        public int Id { get; set; }
        public string Ad { get; set; }

        /// <summary>Ionicons adı; boşsa istemci varsayılan ikon kullanır.</summary>
        public string IkonAnahtari { get; set; }

        public int SiraNo { get; set; }

        public int ToplamKonu { get; set; }
        public int TamamlananKonu { get; set; }
        public List<KonuTakipKonuSatirDto> Konular { get; set; }
    }
}
