using System.Collections.Generic;
using Core.Entities;

namespace Core.Entities.Dtos.Project.KullaniciGunlukSoruCozumuDtos
{
    public class GunlukSoruCozumuPageForMeDto : IDto
    {
        public List<GunlukSoruGunOzetDto> Items { get; set; }
        public int NextOffsetDays { get; set; }
        public bool HasMore { get; set; }
    }
}
