using DIT.Activist.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Domain.Interfaces
{
    public interface IJobIterationNotifier : IObservable<JobIteration>
    {
        void OnLabelsRequested(JobIteration newIteration);

        void LabellingJobComplete();
    }
}
