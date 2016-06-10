using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Domain.Interfaces.Factories
{
    public interface IActivatable
    {
        void Initialize(Dictionary<string, string> parameters);
        IEnumerable<string> ParameterNames { get; }
    }
}
