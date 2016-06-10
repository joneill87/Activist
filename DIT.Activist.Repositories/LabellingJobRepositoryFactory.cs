using DIT.Activist.Domain.Interfaces.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIT.Activist.Domain.Interfaces.Repositories;
using DIT.Activist.Repositories.MemCache;

namespace DIT.Activist.Repositories
{
    public class LabellingJobRepositoryFactory : ILabellingJobRepositoryFactory
    {
        public ILabellingJobRepository Create()
        {
            return new MCLabellingJobRepository();
        }
    }
}
