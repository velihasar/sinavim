namespace Core.Entities.Dtos.Project.KullaniciKonuIlerlemeDtos
{
    public class SetMyKonuIlerlemeDto : IDto
    {
        public int Id { get; set; }
        public int KonuId { get; set; }
        public short Durum { get; set; }
    }
}
