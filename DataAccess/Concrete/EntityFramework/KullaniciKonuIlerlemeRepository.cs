
using System;
using System.Linq;
using Core.DataAccess.EntityFramework;
using Entities.Concrete;
using DataAccess.Concrete.EntityFramework.Contexts;
using DataAccess.Abstract;
using Core.Entities.Concrete.Project;
namespace DataAccess.Concrete.EntityFramework
{
    public class KullaniciKonuIlerlemeRepository : EfEntityRepositoryBase<KullaniciKonuIlerleme, ProjectDbContext>, IKullaniciKonuIlerlemeRepository
    {
        public KullaniciKonuIlerlemeRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}
