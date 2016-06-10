using DIT.Activist.Domain.Interfaces.Factories;
using DIT.Activist.Domain.Interfaces.Repositories;
using DIT.Activist.Repositories.MemCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Repositories
{
    public class JobIterationRepositoryFactory : IJobIterationRepositoryFactory
    {
        public IJobIterationRepository Create()
        {
            return new MCJobIterationRepository();
        }
    }
}
