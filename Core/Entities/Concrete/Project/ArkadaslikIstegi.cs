using System;
using Core.Entities;
using Core.Enums;
namespace Core.Entities.Concrete.Project
{
    /// <summary>
    /// Bir kullanıcının diğerine gönderdiği arkadaşlık isteği.
    /// Kabul edilince <see cref="Arkadaslik"/> kaydı açılır.
    /// </summary>
    public class ArkadaslikIstegi :BaseEntity, IEntity
    {

        public int GonderenUserId { get; set; }
        public User GonderenUser { get; set; }

        public int HedefUserId { get; set; }
        public User HedefUser { get; set; }

        public ArkadaslikIstekDurumu Durum { get; set; }

        /// <summary>İstek hangi davet kodu ile başlatıldı (opsiyonel denetim).</summary>
        public string? KullanilanDavetKodu { get; set; }

        public DateTime OlusturulmaTarihi { get; set; }

        /// <summary>Kabul / red / iptal zamanı.</summary>
        public DateTime? YanitTarihi { get; set; }

        /// <summary>
        /// İstek kabul edildiyse gönderen taraf kabulü gördü mü (rozet için).
        /// Kabul anında false; gönderen Arkadaşlar ekranını açınca true yapılır.
        /// </summary>
        public bool GonderenKabulGordu { get; set; } = true;

        public Arkadaslik? Arkadaslik { get; set; }
    }
}
