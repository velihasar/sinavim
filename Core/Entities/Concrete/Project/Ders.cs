using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Concrete.Project
{
    public class Ders: BaseEntity, IEntity
    {
        public string Ad { get; set; }

        /// <summary>Mobil Ionicons glyph adı (ör. book-outline). Boşsa uygulama varsayılanı kullanır.</summary>
        public string IkonAnahtari { get; set; }

        /// <summary>Listelerde gösterim sırası (küçük önce). Konu.SiraNo ile aynı mantık.</summary>
        public int SiraNo { get; set; }

        public int SinavId { get; set; }
        public Sinav Sinav { get; set; }
        public int? SinavBolumId { get; set; }
        public SinavBolum? SinavBolum { get; set; }
    }
}
