using DIT.Activist.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.ActiveLearning.StoppingCriteria
{
    public class LabelLimit : IStoppingCriterion
    {
        private readonly int maxLabels;

        public LabelLimit(int maxLabels)
        {
            this.maxLabels = maxLabels;
        }
        public Task<bool> ShouldStop(IEnumerable<object[]> unlabelled, IEnumerable<object[]> labelled)
        {
            return Task.FromResult(labelled.Count() >= maxLabels);
        }
    }
}
