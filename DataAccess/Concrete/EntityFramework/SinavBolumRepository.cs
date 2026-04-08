
using System;
using System.Linq;
using Core.DataAccess.EntityFramework;
using Entities.Concrete;
using DataAccess.Concrete.EntityFramework.Contexts;
using DataAccess.Abstract;
using Core.Entities.Concrete.Project;
namespace DataAccess.Concrete.EntityFramework
{
    public class SinavBolumRepository : EfEntityRepositoryBase<SinavBolum, ProjectDbContext>, ISinavBolumRepository
    {
        public SinavBolumRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}
