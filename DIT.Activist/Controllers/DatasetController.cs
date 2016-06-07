using DIT.Activist.Domain.Models.Datasets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DIT.Activist.Webservice.Helpers;
using DIT.Activist.Infrastructure;
using DIT.Activist.Tasks.DataParsing;
using DIT.Activist.Domain.Interfaces;

namespace DIT.Activist.Controllers
{
    public class DatasetController : ApiController
    {
        IDataStoreFactory dsFactory;
        IDataStore dataStore {  get { return dsFactory.Create(CIFAR10Parser.Format); } }

        public DatasetController(IDataStoreFactory dsFactory)
        {
            this.dsFactory = dsFactory;
        }

        public DatasetController() : this(new DataStoreFactory()) { }

        // GET: api/Dataset
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Dataset/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/DataSet
        public async Task<Dataset> Post()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            using (var requestContent = await Request.Content.ParseMultipartAsync())
            {
                if (!requestContent.Files.ContainsKey("file"))
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }

                var file = requestContent.Files["file"];
                var fileName = file.Filename;
                string extension;

                if (!FileHelpers.TryGetExtension(fileName, out extension))
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }

                var fileContent = file.File;

                //need to fix this line to make sure it returns the right file type
                FileHelpers.Filetypes fileType = FileHelpers.GetFileType(extension);
                var parser = new CIFAR10Parser();
               

                var metadata = new DatasetMetadata()
                {
                    Name = "Test Data Set 1"
                };

                dataStore.Clear();

                await dataStore.AddUnlabelledRow(parser.ExtractFeatureValues(fileContent, 500));

                Dataset dataset = new Dataset()
                {
                    Metadata = metadata
                };

                return dataset;
            }
        }

        // PUT: api/Dataset/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Dataset/5
        public void Delete(int id)
        {
        }

        // DELETE: api/Dataset
        public HttpResponseMessage Options()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            return response;
        }
    }
}
