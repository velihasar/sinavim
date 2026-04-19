namespace Core.Entities.Dtos.Project.DenemeSinaviDtos
{
    /// <summary>
    /// Deneme kartında ders bazlı özet (D/Y/B ve net).
    /// </summary>
    public class DenemeSinavSonucOzetDto : IDto
    {
        /// <summary>DenemeSinavSonucu tablo Id; satır yoksa 0.</summary>
        public int Id { get; set; }
        public int DersId { get; set; }
        public string DersAd { get; set; }
        public int DogruSayisi { get; set; }
        public int YanlisSayisi { get; set; }
        public int BosSayisi { get; set; }
        public decimal ToplamNet { get; set; }
    }
}
