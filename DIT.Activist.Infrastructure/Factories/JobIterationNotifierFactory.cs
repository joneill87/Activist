using DIT.Activist.Domain.Interfaces.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIT.Activist.Domain.Interfaces.ActiveLoop;

namespace DIT.Activist.Infrastructure.Factories
{
    public class JobIterationNotifierFactory : IJobIterationNotifierFactory
    {
        public IJobIterationNotifier Create()
        {
            return JobIterationNotifier.Instance;
        }
    }
}
