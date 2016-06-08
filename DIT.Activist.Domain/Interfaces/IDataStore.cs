using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Domain.Interfaces
{
    public interface IDataStore
    {
        Task<IEnumerable<object[]>> GetLabelled();
        Task<IEnumerable<object[]>> GetUnlabelled();
        Task AddLabels(IDictionary<long, string> idLabelLookups);
        Task AddLabelledRow(object[] labelled);
        Task AddUnlabelledRow(object[] unlabelled);
        void Clear();
        Task AddUnlabelledRow(IEnumerable<object[]> unlabelledRows);
        Task AddLabelledRow(IEnumerable<object[]> labelledRows);
        Task<string> GetArtifactById(long id);
        Task<IEnumerable<string>> GetArtifactById(IEnumerable<long> ids);
        Task<object> GetLabelById(long id);
        Task<IEnumerable<object>> GetLabelById(IEnumerable<long> ids);
    }
}
