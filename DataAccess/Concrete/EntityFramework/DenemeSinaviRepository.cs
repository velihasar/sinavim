
using System;
using System.Linq;
using Core.DataAccess.EntityFramework;
using Entities.Concrete;
using DataAccess.Concrete.EntityFramework.Contexts;
using DataAccess.Abstract;
using Core.Entities.Concrete.Project;
namespace DataAccess.Concrete.EntityFramework
{
    public class DenemeSinaviRepository : EfEntityRepositoryBase<DenemeSinavi, ProjectDbContext>, IDenemeSinaviRepository
    {
        public DenemeSinaviRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}
