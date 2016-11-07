using DIT.Activist.Domain.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Infrastructure.Datastores.InMemory
{
    internal class MemCacheDataStore : BaseDataStore
    {
        private static Dictionary<string, Tuple<Cache, IDataFormat>> Datasets = new Dictionary<string, Tuple<Cache, IDataFormat>>();

        private Cache connectedDataset = null;

        protected override Task<object[]> GetItemById(long id)
        {
            if (connectedDataset._unlabelled.ContainsKey(id))
            {
                object[] single = connectedDataset._unlabelled.Single(u => u.Key == id).Value;
                return Task.FromResult(single);
            }
            else
            {
                object[] single = connectedDataset._labelled.Single(l => l.Key == id).Value;
                return Task.FromResult(single);
            }
        }

        public MemCacheDataStore() { }

        public override void Connect(string name)
        {
            if (!Datasets.ContainsKey(name))
            {
                throw new NonExistantDatastoreException(name);
            }
            connectedDataset = Datasets[name].Item1;
            dataFormat = Datasets[name].Item2;
        }

        protected override void CreateDatastore(string name)
        {
            if (Datasets.ContainsKey(name))
            {
                throw new DatastoreExistsException(name);
            }
            CreateOrReplaceDatastore(name);
        }

        protected override void CreateOrReplaceDatastore(string name)
        {
            if (!Datasets.ContainsKey(name))
            {
                Datasets.Add(name, new Tuple<Cache, IDataFormat>(new Cache(), dataFormat));
            }
            Connect(name);
            Clear();
        }

        public override bool Exists(string name)
        {
            return Datasets.ContainsKey(name);
        }

        public override Task AddLabelledRow(object[] labelledRow)
        {
            var id = dataFormat.GetID(labelledRow);
            connectedDataset._labelled.Add(id, labelledRow);
            return Task.FromResult<object>(null);
        }

        public override Task AddUnlabelledRow(object[] unlabelledRow)
        {
            var id = dataFormat.GetID(unlabelledRow);
            if (unlabelledRow.Length < dataFormat.ArrayLength)
            {
                object[] withLabelPadding = new object[dataFormat.ArrayLength];
                Array.Copy(unlabelledRow, withLabelPadding, unlabelledRow.Length);
                connectedDataset._unlabelled.Add(id, withLabelPadding);
            }
            else
            {
                connectedDataset._unlabelled.Add(id, unlabelledRow);
            }

            return Task.FromResult<object>(null);
        }

        public override Task AddLabels(IDictionary<long, string> idLabelLookups)
        {
            foreach (KeyValuePair<long, string> kvp in idLabelLookups)
            {
                long key;
                string label;
                key = kvp.Key;
                label = kvp.Value;
                
                object[] unlabelledRow = connectedDataset._unlabelled[key];
                dataFormat.SetLabel(unlabelledRow, label);

                connectedDataset._unlabelled.Remove(key);
                connectedDataset._labelled.Add(key, unlabelledRow);
            }

            return Task.FromResult<object>(null);
        }

        public override void Clear()
        {
            connectedDataset._labelled.Clear();
            connectedDataset._unlabelled.Clear();
        }

        protected override Task<IEnumerable<object[]>> GetRawLabelledData()
        {
            return Task.FromResult(connectedDataset._labelled.Values.AsEnumerable());
        }

        protected override Task<IEnumerable<object[]>> GetRawUnlabelledData()
        {
            return Task.FromResult(connectedDataset._unlabelled.Values.AsEnumerable());
        }

        private class Cache
        {
            public Dictionary<long, object[]> _labelled;
            public Dictionary<long, object[]> _unlabelled;

            public Cache()
            {
                _labelled = new Dictionary<long, object[]>();
                _unlabelled = new Dictionary<long, object[]>();
            }
        }
    }
}
