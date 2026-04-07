using System;
using Core.Enums;

namespace Core.Entities.Dtos.Project.KullaniciKonuIlerlemeDtos
{
    public class KullaniciKonuIlerlemeListDto : IDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int KonuId { get; set; }
        public IlerlemeDurumu Durum { get; set; }
    }
}
