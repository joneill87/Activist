using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Domain.Interfaces.Factories
{
    public interface IPredictiveModelFactory
    {
        IPredictiveModel Create(string typeName);

        IPredictiveModel Create(string typeName, Dictionary<string, string> parameters);
    }
}
