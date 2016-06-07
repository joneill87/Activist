using DIT.Activist.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Infrastructure.Datastores
{
    public abstract class BaseDataStore : IDataStore
    {
        protected IDataFormat dataFormat;

        internal BaseDataStore(IDataFormat format)
        {
            this.dataFormat = format;
        }

        protected IEnumerable<object> ExtractFeatureValues(object[] fullRow)
        {
            return new ArraySegment<object>(fullRow, dataFormat.FeatureIndexStart, dataFormat.FeatureCount);
        }

        protected IEnumerable<object[]> ExtractFeatureValues(IEnumerable<object[]> fullRows)
        {
            foreach (object[] row in fullRows)
            {
                yield return ExtractFeatureValues(row).ToArray();
            }
        }

        protected IEnumerable<object> ExtractFeaturesAndLabels(object[] fullRow)
        {
            var features = ExtractFeatureValues(fullRow);
            object[] label = new object[] { fullRow[dataFormat.LabelIndex] };
            return features.Concat(label);
        }

        protected IEnumerable<object[]> ExtractFeaturesAndLabels(IEnumerable<object[]> fullRows)
        {
            foreach (var row in fullRows)
            {
                yield return ExtractFeaturesAndLabels(row).ToArray();
            }
        }

        protected abstract Task<IEnumerable<object[]>> GetRawLabelledData();

        protected abstract Task<IEnumerable<object[]>> GetRawUnlabelledData();

        public abstract Task AddLabelledRow(object[] labelled);

        public virtual Task AddLabelledRow(IEnumerable<object[]> labelledRows)
        {
            foreach (object[] row in labelledRows)
            {
                AddLabelledRow(row).Wait();
            }

            return Task.FromResult<object>(null);
        }

        public abstract Task AddLabels(IDictionary<string, string> idLabelLookups);
        public abstract Task AddUnlabelledRow(object[] unlabelled);

        public virtual Task AddUnlabelledRow(IEnumerable<object[]> unlabelledRows)
        {
            foreach (object[] row in unlabelledRows)
            {
                AddUnlabelledRow(row).Wait();
            }
            return Task.FromResult<object>(null);
        }

        public abstract void Clear();
        public virtual Task<IEnumerable<string>> GetArtifactById(IEnumerable<string> ids)
        {
            List<string> artifacts = new List<string>();
            foreach(string id in ids)
            {
                artifacts.Add(GetArtifactById(id).Result);
            }
            return Task.FromResult(artifacts.AsEnumerable());
        }

        public virtual Task<string> GetArtifactById(string id)
        {
            object[] row = GetItemById(id).Result;
            return Task.FromResult(row[dataFormat.ArtifactIndex].ToString());
        }

        public virtual Task<IEnumerable<object[]>> GetFeaturesById(IEnumerable<string> ids)
        {
            List<object[]> featureRows = new List<object[]>();
            foreach (string id in ids)
            {
                featureRows.Add(GetFeaturesById(id).Result);
            }
            return Task.FromResult(featureRows.AsEnumerable());
        }

        public virtual Task<object[]> GetFeaturesById(string id)
        {
            return Task.FromResult(ExtractFeatureValues(GetItemById(id).Result).ToArray());
        }

        protected abstract Task<object[]> GetItemById(string id);

        public virtual Task<object> GetLabelById(string id)
        {
            object[] row = GetItemById(id).Result;
            return Task.FromResult(row[dataFormat.LabelIndex]);
        }

        public virtual Task<IEnumerable<object>> GetLabelById(IEnumerable<string> ids)
        {
            return Task.FromResult(ids.Select(i => GetLabelById(i).Result));
        }

        public virtual Task<IEnumerable<object[]>> GetLabelled()
        {
            return Task.FromResult(ExtractFeaturesAndLabels(GetRawLabelledData().Result));
        }

        public Task<IEnumerable<object[]>> GetUnlabelled()
        {
            return Task.FromResult(ExtractFeatureValues(GetRawUnlabelledData().Result));
        }
    }
}
