using DIT.Activist.Domain.Interfaces;
using DIT.Activist.Infrastructure;
using DIT.Activist.Tasks.DataParsing;
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
        IDataStore dataStore { get { return dsFactory.Create(CIFAR10Parser.Format); } }

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
