using DIT.Activist.Domain.Interfaces.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIT.Activist.Domain.Interfaces;

namespace DIT.Activist.ActiveLearning.Factories
{
    public class StoppingCriterionFactory : BaseFactory<IStoppingCriterion>, IStoppingCriterionFactory
    {
        protected override string RootNamespace
        {
            get
            {
                return "DIT.Activist.ActiveLearning.StoppingCriteria";
            }
        }
    }
}
