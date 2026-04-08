using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Concrete.Project
{
    public class SinavBolum :IEntity
    {
        public int Id { get; set; }
        public int SinavId { get; set; }

        public Sinav Sinav { get; set; }

        public string Isim { get; set; } 
    }
}
