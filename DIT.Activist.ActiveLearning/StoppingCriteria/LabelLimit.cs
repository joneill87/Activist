using DIT.Activist.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.ActiveLearning.StoppingCriteria
{
    internal class LabelLimit : IStoppingCriterion
    {
        private int maxLabels;

        public LabelLimit()
        {
            
        }

        public IEnumerable<string> ParameterNames { get { return new string[] { "maxLabels" }; } }

        public void Initialize(Dictionary<string, string> parameters)
        {
            maxLabels = Convert.ToInt32(parameters["maxLabels"]);
        }

        public Task<bool> ShouldStop(IEnumerable<object[]> unlabelled, IEnumerable<object[]> labelled)
        {
            return Task.FromResult(labelled.Count() >= maxLabels);
        }
    }
}
