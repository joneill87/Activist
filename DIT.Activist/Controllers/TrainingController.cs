using DIT.Activist.Domain.Interfaces.Data;
using DIT.Activist.Infrastructure.Factories;
using DIT.Activist.Tasks.DataParsing.Formats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;

namespace DIT.Activist.Controllers
{
    public class TrainingController : Controller
    {
        IDataStoreFactory dsFactory;
        IDataStore dataStore
        {
            get
            {
                return dsFactory.Create("TestJack", DataFormats.CIFAR10.GetFormat());
            }
        }

        public TrainingController() : this(new DataStoreFactory()) { }

        public TrainingController(IDataStoreFactory dsFactory)
        {
            this.dsFactory = dsFactory;
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}
