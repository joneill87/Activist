using DIT.Activist.Domain.Interfaces;
using DIT.Activist.Domain.Interfaces.Data;
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

        public abstract Task AddLabelledRow(object[] labelled);

        public abstract Task AddLabels(IDictionary<long, string> idLabelLookups);

        public abstract Task AddUnlabelledRow(object[] unlabelled);

        public abstract void Clear();

        public abstract void Connect(string name);

        public abstract bool Exists(string name);

        protected abstract void CreateDatastore(string name);

        protected abstract void CreateOrReplaceDatastore(string name);

        protected abstract Task<object[]> GetItemById(long id);

        protected abstract Task<IEnumerable<object[]>> GetRawLabelledData();

        protected abstract Task<IEnumerable<object[]>> GetRawUnlabelledData();

        public virtual Task AddLabelledRow(IEnumerable<object[]> labelledRows)
        {
            foreach (object[] row in labelledRows)
            {
                AddLabelledRow(row).Wait();
            }

            return Task.FromResult<object>(null);
        }

        public virtual Task AddUnlabelledRow(IEnumerable<object[]> unlabelledRows)
        {
            foreach (object[] row in unlabelledRows)
            {
                AddUnlabelledRow(row).Wait();
            }
            return Task.FromResult<object>(null);
        }

        public virtual Task<string> GetArtifactById(long id)
        {
            object[] row = GetItemById(id).Result;
            return Task.FromResult(dataFormat.GetArtifact(row));
        }

        public virtual Task<IEnumerable<string>> GetArtifactById(IEnumerable<long> ids)
        {
            List<string> artifacts = new List<string>();
            foreach (long id in ids)
            {
                artifacts.Add(GetArtifactById(id).Result);
            }
            return Task.FromResult(artifacts.AsEnumerable());
        }

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

        public virtual Task<IEnumerable<object[]>> GetUnlabelled()
        {
            return Task.FromResult(GetRawUnlabelledData().Result);
        }

        public void Create(string name, IDataFormat dataFormat)
        {
            this.dataFormat = dataFormat;
            CreateDatastore(name);
        }

        public void CreateOrReplace(string name, IDataFormat dataFormat)
        {
            this.dataFormat = dataFormat;
            CreateOrReplaceDatastore(name);
        }

        public void CreateOrConnect(string name, IDataFormat dataFormat)
        {
            this.dataFormat = dataFormat;
            if (!Exists(name))
            {
                CreateDatastore(name);
            }
            else
            {
                Connect(name);
            }
        }
    }
}
