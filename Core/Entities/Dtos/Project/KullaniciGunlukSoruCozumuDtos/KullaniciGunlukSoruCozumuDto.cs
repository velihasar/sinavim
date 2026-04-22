using System;
using Core.Entities;

namespace Core.Entities.Dtos.Project.KullaniciGunlukSoruCozumuDtos
{
    public class KullaniciGunlukSoruCozumuDto : IDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Tarih { get; set; }
        public int CozulenSoruSayisi { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
