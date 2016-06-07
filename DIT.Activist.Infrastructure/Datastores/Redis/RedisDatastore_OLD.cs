//using DIT.Activist.Domain.Interfaces;
//using StackExchange.Redis;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DIT.Activist.Infrastructure.Datastores.Redis
//{
//    public class RedisDatastore : IDataStore
//    {
//        public RedisKey RootNamespace { get; private set; }
//        public string Name { get; private set; }
//        private IDataFormat format;

//        private RedisKey GetRedisID(string id)
//        {
//            return RootNamespace.Append("/" + id);
//        }

//        private RedisKey GetLabelledIndexKey()
//        {
//            return RootNamespace.Append("_labelled");
//        }

//        private RedisKey GetUnlabelledIndexKey()
//        {
//            return RootNamespace.Append("_unlabelled");
//        }

//        internal RedisDatastore(RedisKey rootNamespace, IDataFormat format)
//        {
//            this.RootNamespace = rootNamespace;
//            string sRootNamespace = rootNamespace;
//            this.Name = sRootNamespace.Substring(sRootNamespace.LastIndexOf("/") + 1);
//            this.format = format;
//        }

//        private object[] ConvertToTypedRow(RedisValue[] values)
//        {
//            return values.Select(v => v.ToString()).ToArray();
//        }

//        private async Task<object[]> ConvertToTypedRow(Task<RedisValue[]> values)
//        {
//            return ConvertToTypedRow(await values);
//        }

//        private async Task DeleteLabelledRow(string id)
//        {
//            var db = RedisDatabase.GetDatabase();
//            var idKey = GetRedisID(id);
//            await db.KeyDeleteAsync(idKey);
//            await db.SetRemoveAsync(GetLabelledIndexKey(), id);
//        }

//        private async Task DeleteUnlabelledRow(string id)
//        {
//            var db = RedisDatabase.GetDatabase();
//            var idKey = GetRedisID(id);
//            await db.KeyDeleteAsync(idKey);
//            await db.SetRemoveAsync(GetUnlabelledIndexKey(), id);
//        }

//        private Task<IEnumerable<object[]>> GetRowsFromIndex(RedisKey indexKey)
//        {
//            var db = RedisDatabase.GetDatabase();
//            return db.SetMembersAsync(indexKey).ContinueWith(
//                (labelledKeysTask) =>
//                {
//                    var labelledKeys = labelledKeysTask.Result.Select(k => k.ToString());
//                    return labelledKeys.Select(k => ConvertToTypedRow(db.ListRange(GetRedisID(k))));
//                });
//        }

//        public async Task AddLabelledRow(object[] data)
//        {
//            string id = data[0].ToString();
//            //we assume by convention that ID is the first element in the array
//            RedisKey idKey = GetRedisID(id);
//            //the remaining information is the list of feature values
//            IEnumerable<RedisValue> featureValues = data.Select(d => (RedisValue)(d.ToString()));

//            var db = RedisDatabase.GetDatabase();
//            //create a list using ListRightPush to store the data
//            long listLength = await db.ListRightPushAsync(idKey, featureValues.ToArray());
//            await db.SetAddAsync(GetLabelledIndexKey(), id);
//        }

//        public async Task AddUnlabelledRow(object[] data)
//        {
//            string id = data[0].ToString();
//            RedisKey idKey = GetRedisID(id);
//            IEnumerable<RedisValue> featureValues = data.Select(d => (RedisValue)(d.ToString()));

//            var db = RedisDatabase.GetDatabase();
//            long listLength = await db.ListRightPushAsync(idKey, featureValues.ToArray());
//            await db.SetAddAsync(GetUnlabelledIndexKey(), id);
//        }

//        public Task<IEnumerable<object[]>> GetLabelled()
//        {
//            return GetRowsFromIndex(GetLabelledIndexKey());
//        }

//        public Task<IEnumerable<object[]>> GetUnlabelled()
//        {
//            return GetRowsFromIndex(GetUnlabelledIndexKey());
//        }

//        public async Task AddLabels(IDictionary<string, string> idLabelLookups)
//        {
//            var db = RedisDatabase.GetDatabase();

//            foreach (var kvp in idLabelLookups)
//            {
//                string id = kvp.Key;
//                string label = kvp.Value;
//                RedisKey idKey = GetRedisID(id);
//                await db.ListRightPushAsync(idKey, label);
//                await db.SetRemoveAsync(GetUnlabelledIndexKey(), id);
//                await db.SetAddAsync(GetLabelledIndexKey(), id);
//            }
//        }

//        public async Task<object[]> GetFeaturesById(string id)
//        {
//            var db = RedisDatabase.GetDatabase();
//            return ConvertToTypedRow(await db.ListRangeAsync(GetRedisID(id)));
//        }

//        public async Task<IEnumerable<object[]>> GetFeaturesById(IEnumerable<string> ids)
//        {
//            var db = RedisDatabase.GetDatabase();
//            List<object[]> rows = new List<object[]>();
//            foreach (string id in ids)
//            {
//                rows.Add(await GetFeaturesById(id));
//            }
//            return rows;
//        }

//        public void Clear()
//        {
//            RedisDatabase.DeleteDatabase();
//        }

//        public async Task AddUnlabelledRow(IEnumerable<object[]> unlabelledRows)
//        {
//            foreach (object[] row in unlabelledRows)
//            {
//                await AddUnlabelledRow(row);
//            }
//        }

//        public async Task AddLabelledRow(IEnumerable<object[]> labelledRows)
//        {
//            foreach(object[] row in labelledRows)
//            {
//                await AddLabelledRow(row);
//            }
//        }

//        public Task<string> GetArtifactById(string id)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<IEnumerable<string>> GetArtifactById(IEnumerable<string> ids)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<object> GetLabelById(string id)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<IEnumerable<object>> GetLabelById(IEnumerable<string> ids)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
