using System;

namespace Core.Entities.Dtos.Project.DenemeSinavSonucuDtos
{
    public class DenemeSinavSonucuDto : IDto
    {
        public int Id { get; set; }
        public int DenemeSinaviId { get; set; }
        public int DersId { get; set; }
        public int DogruSayisi { get; set; }
        public int YanlisSayisi { get; set; }
        public int BosSayisi { get; set; }
        public decimal ToplamNet { get; set; }
    }
}
