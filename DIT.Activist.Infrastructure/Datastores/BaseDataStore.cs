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

        public abstract Task AddLabels(IDictionary<long, string> idLabelLookups);
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

        public virtual Task<IEnumerable<string>> GetArtifactById(IEnumerable<long> ids)
        {
            List<string> artifacts = new List<string>();
            foreach(long id in ids)
            {
                artifacts.Add(GetArtifactById(id).Result);
            }
            return Task.FromResult(artifacts.AsEnumerable());
        }

        public virtual Task<string> GetArtifactById(long id)
        {
            object[] row = GetItemById(id).Result;
            return Task.FromResult(dataFormat.GetArtifact(row));
        }

        protected abstract Task<object[]> GetItemById(long id);

        public virtual Task<object> GetLabelById(long id)
        {
            object[] row = GetItemById(id).Result;
            return Task.FromResult(dataFormat.GetLabel<object>(row));
        }

        public virtual Task<IEnumerable<object>> GetLabelById(IEnumerable<long> ids)
        {
            return Task.FromResult(ids.Select(i => GetLabelById(i).Result));
        }

        public virtual Task<IEnumerable<object[]>> GetLabelled()
        {
            return Task.FromResult(GetRawLabelledData().Result);
        }

        public Task<IEnumerable<object[]>> GetUnlabelled()
        {
            return Task.FromResult(GetRawUnlabelledData().Result);
        }
    }
}
