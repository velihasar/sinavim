using System;
using Core.Entities;

namespace Core.Entities.Concrete.Project
{
    /// <summary>
    /// Kabul edilmiş arkadaşlık (çift yönlü tek satır).
    /// UserIdKucuk her zaman UserIdBuyuk değerinden küçük tutulur; çift kayıt önlenir.
    /// </summary>
    public class Arkadaslik : BaseEntity,IEntity
    {

        public int UserIdKucuk { get; set; }
        public User UserKucuk { get; set; }

        public int UserIdBuyuk { get; set; }
        public User UserBuyuk { get; set; }

        public int? ArkadaslikIstegiId { get; set; }
        public ArkadaslikIstegi? ArkadaslikIstegi { get; set; }

        public DateTime OlusturulmaTarihi { get; set; }
    }
}
