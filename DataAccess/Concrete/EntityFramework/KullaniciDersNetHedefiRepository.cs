
using System;
using System.Linq;
using Core.DataAccess.EntityFramework;
using Entities.Concrete;
using DataAccess.Concrete.EntityFramework.Contexts;
using DataAccess.Abstract;
using Core.Entities.Concrete.Project;
namespace DataAccess.Concrete.EntityFramework
{
    public class KullaniciDersNetHedefiRepository : EfEntityRepositoryBase<KullaniciDersNetHedefi, ProjectDbContext>, IKullaniciDersNetHedefiRepository
    {
        public KullaniciDersNetHedefiRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}
