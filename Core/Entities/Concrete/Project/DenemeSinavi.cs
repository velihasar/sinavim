using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Concrete.Project
{
    public class DenemeSinavi:BaseEntity,IEntity
    {
        public string Ad { get; set; }
        public string Aciklama { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int SinavId { get; set; }
        public Sinav Sinav { get; set; }
        public DateTime Tarih { get; set; }
        
    }
}
