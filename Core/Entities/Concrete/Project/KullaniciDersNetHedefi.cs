namespace Core.Entities.Concrete.Project
{
    /// <summary>
    /// Kullanıcının bir ders için deneme sınavlarında tutturmayı hedeflediği net (belirli bir denemeye bağlı değil).
    /// DenemeSinavSonucu.ToplamNet ile karşılaştırılır.
    /// </summary>
    public class KullaniciDersNetHedefi :BaseEntity, IEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int DersId { get; set; }
        public Ders Ders { get; set; }

        /// <summary>Bölümlü sınavlarda filtreleme; Ders.SinavBolumId ile uyumlu olmalıdır.</summary>
        public int? SinavBolumId { get; set; }
        public SinavBolum? SinavBolum { get; set; }

        public decimal HedefNet { get; set; }
    }
}
