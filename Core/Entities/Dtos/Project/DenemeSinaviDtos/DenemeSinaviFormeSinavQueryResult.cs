using System.Collections.Generic;

namespace Core.Entities.Dtos.Project.DenemeSinaviDtos
{
    /// <summary>
    /// formesinav yanıtı: liste + (sayfalı istekte) toplam kayıt sayısı.
    /// </summary>
    public class DenemeSinaviFormeSinavQueryResult : IDto
    {
        public List<DenemeSinaviOzetListDto> Items { get; set; }
        /// <summary>
        /// Sayfalı istekte filtreye uyan toplam; diğer durumda 0.
        /// </summary>
        public int TotalCount { get; set; }
    }
}
