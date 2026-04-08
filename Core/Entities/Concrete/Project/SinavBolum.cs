using Core.Entities;

namespace Core.Entities.Concrete.Project
{
    public class SinavBolum : BaseEntity, IEntity
    {
        public int SinavId { get; set; }

        public Sinav Sinav { get; set; }

        public string Isim { get; set; }
    }
}
