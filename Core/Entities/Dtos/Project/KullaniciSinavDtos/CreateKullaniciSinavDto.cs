using System;

namespace Core.Entities.Dtos.Project.KullaniciSinavDtos
{
    public class CreateKullaniciSinavDto : IDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SinavId { get; set; }
        public int HedefPuan { get; set; }
    }
}
