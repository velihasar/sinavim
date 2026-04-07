using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Concrete.Project
{
    public class KullaniciSinav: BaseEntity, IEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int SinavId { get; set; }
        public Sinav Sinav { get; set; }
        public int HedefPuan { get; set; }
    }
}
