using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Domain.Interfaces
{
    public interface IStoppingCriterion
    {
        Task<bool> ShouldStop(IEnumerable<object[]> unlabelled, IEnumerable<object[]> labelled);
    }
}
