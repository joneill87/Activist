using DIT.Activist.Domain.Interfaces;
using DIT.Activist.Infrastructure;
using DIT.Activist.Infrastructure.Factories;
using DIT.Activist.Tasks.DataParsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DIT.Activist.Controllers
{
    public class ArtifactsController : ApiController
    {
        private IDataStoreFactory dataStoreFactory;

        private IDataStore dataStore { get { return dataStoreFactory.Create(CIFAR10Parser.Format); } }

        public ArtifactsController(IDataStoreFactory dsFactory)
        {
            dataStoreFactory = dsFactory;
        }

        public ArtifactsController() : this(new DataStoreFactory()) { }

        // GET: api/Artifacts?ids=53443-286693...
        public async Task<IEnumerable<string>> Get([FromUri]long[] ids)
        {
            return await dataStore.GetArtifactById(ids);
        }

        // POST: api/Artifacts
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Artifacts/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Artifacts/5
        public void Delete(int id)
        {
        }
    }
}
