using DIT.Activist.Domain.Interfaces.ActiveLoop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Domain.Interfaces.Factories
{
    public interface IJobIterationNotifierFactory
    {
        IJobIterationNotifier Create();
    }
}
