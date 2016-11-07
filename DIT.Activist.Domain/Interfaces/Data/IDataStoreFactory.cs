using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Domain.Interfaces.Data
{
    public interface IDataStoreFactory
    {
        IDataStore Create(string name, IDataFormat format);

        IDataStore CreateOrReplace(string name, IDataFormat format);

        IDataStore CreateOrConnect(string name, IDataFormat format);

        IDataStore Retrieve(string name);
    }
}
