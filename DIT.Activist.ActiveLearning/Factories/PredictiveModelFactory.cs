using DIT.Activist.Domain.Interfaces;
using DIT.Activist.Domain.Interfaces.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.ActiveLearning.Factories
{
    public class PredictiveModelFactory : BaseFactory<IPredictiveModel>, IPredictiveModelFactory
    {
        private const string ROOT_NAMESPACE = "DIT.Activist.ActiveLearning.Models";

        protected override string RootNamespace
        {
            get
            {
                return ROOT_NAMESPACE;
            }
        }
    }
}
