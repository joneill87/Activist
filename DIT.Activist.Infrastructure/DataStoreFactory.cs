using DIT.Activist.Domain.Interfaces;
using DIT.Activist.Infrastructure.Datastores.InMemory;
using DIT.Activist.Infrastructure.Datastores.Redis;
using DIT.Activist.Infrastructure.Datastores.SQLServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Infrastructure
{
    public class DataStoreFactory : IDataStoreFactory
    {
        public IDataStore Create(IDataFormat format)
        {
            return new MemCacheDataStore(format);
        }
    }
}
