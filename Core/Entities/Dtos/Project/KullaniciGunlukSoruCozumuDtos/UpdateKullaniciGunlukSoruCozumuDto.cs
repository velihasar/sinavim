using System;
using Core.Entities;

namespace Core.Entities.Dtos.Project.KullaniciGunlukSoruCozumuDtos
{
    public class UpdateKullaniciGunlukSoruCozumuDto : IDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Tarih { get; set; }
        public int CozulenSoruSayisi { get; set; }
    }
}
