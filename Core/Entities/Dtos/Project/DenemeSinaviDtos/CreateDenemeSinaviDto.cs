using System;

namespace Core.Entities.Dtos.Project.DenemeSinaviDtos
{
    public class CreateDenemeSinaviDto : IDto
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public string Aciklama { get; set; }
        public int UserId { get; set; }
        public int SinavId { get; set; }
        public int? SinavBolumId { get; set; }
        public DateTime Tarih { get; set; }
    }
}
