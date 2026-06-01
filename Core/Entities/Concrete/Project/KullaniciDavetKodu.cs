using System;
using Core.Entities;

namespace Core.Entities.Concrete.Project
{
    /// <summary>
    /// Kullanıcının arkadaş davet kodu (tek kayıt / kullanıcı).
    /// Kod ile istek gönderilir; kabul sonrası <see cref="Arkadaslik"/> oluşur.
    /// </summary>
    public class KullaniciDavetKodu : IEntity
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        /// <summary>Benzersiz davet kodu (ör. 6–8 karakter).</summary>
        public string Kod { get; set; }

        public DateTime OlusturulmaTarihi { get; set; }
    }
}
