using System;
using Core.Entities;

namespace Core.Entities.Dtos.Project.ArkadaslikDtos
{
    public class KullaniciDavetKoduDto : IDto
    {
        public string Kod { get; set; }
        public DateTime OlusturulmaTarihi { get; set; }
    }
}
