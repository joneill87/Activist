using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Infrastructure.Datastores
{
    public class DatastoreExistsException : Exception
    {
        public DatastoreExistsException(string name) : base("A datastore already exists with this name: " + name)
        { }
    }

    public class NonExistantDatastoreException : Exception
    {
        public NonExistantDatastoreException(string name) : base("Attempt to connect to non-existant datastore: " + name)
            { }
    }
}
