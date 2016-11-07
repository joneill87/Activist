using DIT.Activist.Domain.Interfaces.Data;
using DIT.Activist.Infrastructure.Datastores;
using DIT.Activist.Infrastructure.Datastores.InMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Infrastructure.Factories
{
    public class DataStoreFactory : IDataStoreFactory
    {
        private BaseDataStore GetInstance()
        {
            return new MemCacheDataStore();
        }

        public IDataStore Create(string name, IDataFormat format)
        {
            var ds = GetInstance();
            ds.Create(name, format);
            ds.Connect(name);
            return ds;
        }

        public IDataStore CreateOrReplace(string name, IDataFormat format)
        {
            var ds = GetInstance();
            ds.CreateOrReplace(name, format);
            ds.Connect(name);
            return ds;
        }

        public IDataStore CreateOrConnect(string name, IDataFormat format)
        {
            var ds = GetInstance();
            ds.CreateOrConnect(name, format);
            return ds;
        }

        public IDataStore Retrieve(string name)
        {
            var ds = GetInstance();
            ds.Connect(name);
            return ds;
        }
    }
}
