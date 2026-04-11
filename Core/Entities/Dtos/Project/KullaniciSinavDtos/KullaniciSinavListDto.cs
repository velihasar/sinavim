using System;

namespace Core.Entities.Dtos.Project.KullaniciSinavDtos
{
    public class KullaniciSinavListDto : IDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SinavId { get; set; }
        public int HedefPuan { get; set; }

        /// <summary>İlişkili sınavın tarihi (geri sayım için).</summary>
        public DateTime? SinavTarih { get; set; }

        public string? SinavKisaAd { get; set; }
        public string? SinavAd { get; set; }
    }
}
