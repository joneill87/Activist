using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Domain.Interfaces.Factories
{
    public interface ISelectionStrategyFactory
    {
        ISelectionStrategy Create(string typeName);

        ISelectionStrategy Create(string typeName, Dictionary<string, string> parameters);
    }
}
