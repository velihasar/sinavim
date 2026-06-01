using System.Collections.Generic;
using Core.Entities;

namespace Core.Entities.Dtos.Project.ArkadaslikDtos
{
    public class MyArkadaslikIstekleriDto : IDto
    {
        public IList<ArkadaslikIstegiListItemDto> Gelen { get; set; } = new List<ArkadaslikIstegiListItemDto>();
        public IList<ArkadaslikIstegiListItemDto> Giden { get; set; } = new List<ArkadaslikIstegiListItemDto>();
    }
}
