using System;
using Core.Entities;

namespace Core.Entities.Dtos.Project.KullaniciGunlukSoruCozumuDtos
{
    public class GunlukSoruGunOzetDto : IDto
    {
        public DateTime Tarih { get; set; }
        public int CozulenSoruSayisi { get; set; }
    }
}
