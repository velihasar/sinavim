using System;
using Core.Entities;

namespace Core.Entities.Dtos.Project.ArkadaslikDtos
{
    public class ArkadasOzetDto : IDto
    {
        public int ArkadaslikId { get; set; }
        public int ArkadasUserId { get; set; }
        public string ArkadasFullName { get; set; }
        public DateTime OlusturulmaTarihi { get; set; }
    }
}
