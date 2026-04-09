using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Concrete.Project
{
    public class Sinav : BaseEntity, IEntity
    {
        public string KısaAd { get; set; }
        public string Ad { get; set; }
        public string? Aciklama { get; set; }
        public DateTime Tarih { get; set; }
        public int Index { get; set; }
        public int DogruyuGoturenYanlisSay { get; set; }

    }
}
