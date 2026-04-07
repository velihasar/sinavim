using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Enums;

namespace Core.Entities.Concrete.Project
{
    public class KullaniciKonuIlerleme:IEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int KonuId { get; set; }
        public Konu Konu { get; set; }
        public IlerlemeDurumu Durum { get; set; } // 0 = başlamadı, 1 = çalışıyor, 2 = tamamladı

    }
}
