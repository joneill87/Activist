using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Domain.Interfaces
{
    public interface ISeedingStrategy
    {
        Task<IEnumerable<string>> GetQueryIDs(IDataStore dataStore, int seedSize);
    }
}
