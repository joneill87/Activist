using DIT.Activist.Domain.Models.Datasets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DIT.Activist.Webservice.Helpers;
using DIT.Activist.Tasks.DataParsing.Formats;

using DIT.Activist.Infrastructure.Factories;
using DIT.Activist.Domain.Interfaces.Data;
using DIT.Activist.Tasks.DataParsing.Parsers;
using DIT.Activist.Tasks.DataParsing;

namespace DIT.Activist.Controllers
{
    public class DatasetController : ApiController
    {
        IDataStoreFactory dsFactory;
        IDataParserFactory parserFactory;

        IDataStore GetDataStore(string datastoreName, IDataFormat format)
        {
            return dsFactory.CreateOrConnect(datastoreName, format);
        }

        public DatasetController(IDataStoreFactory dsFactory, IDataParserFactory parserFactory)
        {
            this.dsFactory = dsFactory;
            this.parserFactory = parserFactory;
        }

        public DatasetController() : this(new DataStoreFactory(), new DataParserFactory()) { }

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
        public async Task<Dataset> Post([FromUri]string datasetName, [FromUri]DataFormats dataFormat)
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

                var parser = parserFactory.Create(dataFormat);
               

                var metadata = new DatasetMetadata()
                {
                    Name = "Test Data Set 1"
                };

                var dataStore = GetDataStore(datasetName, dataFormat.GetFormat());
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
