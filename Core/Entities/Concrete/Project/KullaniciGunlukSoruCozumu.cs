using System;
using Core.Entities.Concrete;

namespace Core.Entities.Concrete.Project
{
    public class KullaniciGunlukSoruCozumu : BaseEntity, IEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime Tarih { get; set; }
        public int CozulenSoruSayisi { get; set; }
    }
}
