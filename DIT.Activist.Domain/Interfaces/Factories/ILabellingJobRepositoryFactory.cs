using DIT.Activist.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Domain.Interfaces.Factories
{
    public interface ILabellingJobRepositoryFactory
    {
        ILabellingJobRepository Create();
    }
}
