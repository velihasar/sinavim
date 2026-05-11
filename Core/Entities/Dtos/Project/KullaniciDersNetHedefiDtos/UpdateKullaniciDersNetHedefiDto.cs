using Core.Entities;

namespace Core.Entities.Dtos.Project.KullaniciDersNetHedefiDtos
{
    public class UpdateKullaniciDersNetHedefiDto : IDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int DersId { get; set; }
        public int? SinavBolumId { get; set; }
        public decimal HedefNet { get; set; }
    }
}
