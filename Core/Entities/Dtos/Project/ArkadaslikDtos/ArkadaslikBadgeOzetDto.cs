using Core.Entities;

namespace Core.Entities.Dtos.Project.ArkadaslikDtos
{
    public class ArkadaslikBadgeOzetDto : IDto
    {
        public int GelenBekleyen { get; set; }
        public int GidenBekleyen { get; set; }
        /// <summary>Gönderen olarak henüz görülmemiş kabul edilmiş istekler.</summary>
        public int KabulGorulmedi { get; set; }
    }
}
