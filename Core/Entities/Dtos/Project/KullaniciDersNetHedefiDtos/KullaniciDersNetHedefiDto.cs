using System;
using Core.Entities;
using Core.Entities.Concrete.Project;

namespace Core.Entities.Dtos.Project.KullaniciDersNetHedefiDtos
{
    public class KullaniciDersNetHedefiDto : IDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int DersId { get; set; }
        public int? SinavBolumId { get; set; }
        public decimal HedefNet { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public static KullaniciDersNetHedefiDto FromEntity(KullaniciDersNetHedefi e)
        {
            if (e == null)
            {
                return null;
            }

            return new KullaniciDersNetHedefiDto
            {
                Id = e.Id,
                UserId = e.UserId,
                DersId = e.DersId,
                SinavBolumId = e.SinavBolumId,
                HedefNet = e.HedefNet,
                CreatedDate = e.CreatedDate,
                UpdatedDate = e.UpdatedDate,
                IsActive = e.IsActive,
            };
        }
    }
}
