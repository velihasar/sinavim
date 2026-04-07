
using System;
using System.Linq;
using Core.DataAccess.EntityFramework;
using Entities.Concrete;
using DataAccess.Concrete.EntityFramework.Contexts;
using DataAccess.Abstract;
using Core.Entities.Concrete.Project;
namespace DataAccess.Concrete.EntityFramework
{
    public class DenemeSinavSonucuRepository : EfEntityRepositoryBase<DenemeSinavSonucu, ProjectDbContext>, IDenemeSinavSonucuRepository
    {
        public DenemeSinavSonucuRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}
