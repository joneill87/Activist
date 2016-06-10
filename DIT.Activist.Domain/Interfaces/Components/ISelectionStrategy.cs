using DIT.Activist.Domain.Interfaces.Data;
using DIT.Activist.Domain.Interfaces.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Domain.Interfaces
{
    public interface ISelectionStrategy : IActivatable
    {
        Task<ICollection<long>> GenerateQuery(IEnumerable<object[]> labelled, IEnumerable<object[]> unlabelled, IDataFormat format, int batchSize);
    }
}
