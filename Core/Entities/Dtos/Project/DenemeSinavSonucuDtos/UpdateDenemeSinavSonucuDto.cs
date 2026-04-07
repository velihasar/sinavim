using System;

namespace Core.Entities.Dtos.Project.DenemeSinavSonucuDtos
{
    public class UpdateDenemeSinavSonucuDto : IDto
    {
        public int Id { get; set; }
        public int DersId { get; set; }
        public int DogruSayisi { get; set; }
        public int YanlisSayisi { get; set; }
        public int BosSayisi { get; set; }
        public decimal ToplamNet { get; set; }
    }
}
