
using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Contexts;
using Core.Entities.Concrete.Project;

namespace DataAccess.Concrete.EntityFramework
{
    public class KullaniciDavetKoduRepository
        : EfEntityRepositoryBase<KullaniciDavetKodu, ProjectDbContext>, IKullaniciDavetKoduRepository
    {
        public KullaniciDavetKoduRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}
