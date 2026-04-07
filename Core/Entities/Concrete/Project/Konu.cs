using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Concrete.Project
{
    public class Konu: BaseEntity, IEntity
    {
        public string Ad { get; set; }
        public int SiraNo { get; set; }
        public int DersId { get; set; }
        public Ders Ders { get; set; }
    }
}
