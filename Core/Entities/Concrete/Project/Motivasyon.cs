using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Concrete.Project
{
    public class Motivasyon :BaseEntity,IEntity
    {
        public string Kelime { get; set; }
    }
}
