using DIT.Activist.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Domain.Models
{
    public class LabellingJob
    {
        public Guid JobID { get; set; }

        public IDataStore DataStore { get; set; }

        public ISelectionStrategy SelectionStrategy { get; set; }

        public int BatchSize { get; set; }

        public IPredictiveModel Model { get; set; }

        public IStoppingCriterion StoppingCriterion { get; set; }
    }
}
