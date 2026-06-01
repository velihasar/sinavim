using System;
using Core.Entities;
using Core.Enums;

namespace Core.Entities.Dtos.Project.ArkadaslikDtos
{
    public class ArkadaslikIstegiListItemDto : IDto
    {
        public int Id { get; set; }
        public int GonderenUserId { get; set; }
        public string GonderenFullName { get; set; }
        public int HedefUserId { get; set; }
        public string HedefFullName { get; set; }
        public ArkadaslikIstekDurumu Durum { get; set; }
        public DateTime OlusturulmaTarihi { get; set; }
        public DateTime? YanitTarihi { get; set; }
    }
}
