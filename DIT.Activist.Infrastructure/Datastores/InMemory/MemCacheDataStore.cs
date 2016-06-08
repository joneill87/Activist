using DIT.Activist.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Infrastructure.Datastores.InMemory
{
    public class MemCacheDataStore : BaseDataStore
    {
        private static readonly Dictionary<long, object[]> _labelled = new Dictionary<long, object[]>();
        private static readonly Dictionary<long, object[]> _unlabelled = new Dictionary<long, object[]>();

        protected override Task<object[]> GetItemById(long id)
        {
            if (_unlabelled.ContainsKey(id))
            {
                object[] single = _unlabelled.Single(u => u.Key == id).Value;
                return Task.FromResult(single);
            }
            else
            {
                object[] single = _labelled.Single(l => l.Key == id).Value;
                return Task.FromResult(single);
            }
        }

        public MemCacheDataStore(IDataFormat dataFormat) : base(dataFormat)
        {

        }


        public override Task AddLabelledRow(object[] labelledRow)
        {
            var id = dataFormat.GetID(labelledRow);
            _labelled.Add(id, labelledRow);
            return Task.FromResult<object>(null);
        }

        public override Task AddUnlabelledRow(object[] unlabelledRow)
        {
            var id = dataFormat.GetID(unlabelledRow);
            if (unlabelledRow.Length < dataFormat.ArrayLength)
            {
                object[] withLabelPadding = new object[dataFormat.ArrayLength];
                Array.Copy(unlabelledRow, withLabelPadding, unlabelledRow.Length);
                _unlabelled.Add(id, withLabelPadding);
            }
            else
            {
                _unlabelled.Add(id, unlabelledRow);
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
                
                object[] unlabelledRow = _unlabelled[key];
                dataFormat.SetLabel(unlabelledRow, label);

                _unlabelled.Remove(key);
                _labelled.Add(key, unlabelledRow);
            }

            return Task.FromResult<object>(null);
        }

        public override void Clear()
        {
            _labelled.Clear();
            _unlabelled.Clear();
        }

        protected override Task<IEnumerable<object[]>> GetRawLabelledData()
        {
            return Task.FromResult(_labelled.Values.AsEnumerable());
        }

        protected override Task<IEnumerable<object[]>> GetRawUnlabelledData()
        {
            return Task.FromResult(_unlabelled.Values.AsEnumerable());
        }
    }
}
