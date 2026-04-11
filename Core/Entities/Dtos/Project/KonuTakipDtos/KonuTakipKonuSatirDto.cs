namespace Core.Entities.Dtos.Project.KonuTakipDtos
{
    public class KonuTakipKonuSatirDto : IDto
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public int SiraNo { get; set; }

        /// <summary>0 başlamadı, 1 çalışıyor, 2 tamamlandı (IlerlemeDurumu).</summary>
        public short Durum { get; set; }

        public int? IlerlemeKayitId { get; set; }
    }
}
