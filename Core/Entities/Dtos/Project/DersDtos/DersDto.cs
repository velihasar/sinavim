using System;

namespace Core.Entities.Dtos.Project.DersDtos
{
    public class DersDto : IDto
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public int SinavId { get; set; }
    }
}
