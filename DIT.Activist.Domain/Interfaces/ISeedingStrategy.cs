using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Domain.Interfaces
{
    public interface ISeedingStrategy
    {
        Task<IEnumerable<long>> GetQueryIDs(IDataStore dataStore, IDataFormat format, int seedSize);
    }
}
