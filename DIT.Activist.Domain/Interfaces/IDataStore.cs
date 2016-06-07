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
        Task AddLabels(IDictionary<string, string> idLabelLookups);
        Task AddLabelledRow(object[] labelled);
        Task AddUnlabelledRow(object[] unlabelled);
        Task<object[]> GetFeaturesById(string id);
        Task<IEnumerable<object[]>> GetFeaturesById(IEnumerable<string> ids);
        void Clear();
        Task AddUnlabelledRow(IEnumerable<object[]> unlabelledRows);
        Task AddLabelledRow(IEnumerable<object[]> labelledRows);
        Task<string> GetArtifactById(string id);
        Task<IEnumerable<string>> GetArtifactById(IEnumerable<string> ids);
        Task<object> GetLabelById(string id);
        Task<IEnumerable<object>> GetLabelById(IEnumerable<string> ids);
    }
}
