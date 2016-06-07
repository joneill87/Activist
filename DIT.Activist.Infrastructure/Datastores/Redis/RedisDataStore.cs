using DIT.Activist.Domain.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Infrastructure.Datastores.Redis
{
    public class RedisDataStore : BaseDataStore
    {
        public RedisKey RootNamespace { get; private set; }

        public RedisDataStore(RedisKey rootNamespace, IDataFormat dataFormat) : base(dataFormat)
        {
            RootNamespace = rootNamespace;
        }
       

        private RedisKey GetRedisID(string id)
        {
            return RootNamespace.Append("/" + id);
        }

        private RedisKey GetLabelledIndexKey()
        {
            return RootNamespace.Append("_labelled");
        }

        private RedisKey GetUnlabelledIndexKey()
        {
            return RootNamespace.Append("_unlabelled");
        }

        private object[] ConvertToTypedRow(RedisValue[] values)
        {
            return values.Select(v => v.ToString()).ToArray();
        }

        private Task<IEnumerable<object[]>> GetRowsFromIndex(RedisKey indexKey)
        {
            var db = RedisDatabase.GetDatabase();
            return db.SetMembersAsync(indexKey).ContinueWith(
                (labelledKeysTask) =>
                {
                    var labelledKeys = labelledKeysTask.Result.Select(k => k.ToString());
                    return labelledKeys.Select(k => ConvertToTypedRow(db.ListRange(GetRedisID(k))));
                });
        }

        public override async Task AddLabelledRow(object[] labelled)
        {
            string id = labelled[0].ToString();
            //we assume by convention that ID is the first element in the array
            RedisKey idKey = GetRedisID(id);
            //the remaining information is the list of feature values
            IEnumerable<RedisValue> featureValues = labelled.Select(d => (RedisValue)(d.ToString()));

            var db = RedisDatabase.GetDatabase();
            //create a list using ListRightPush to store the data
            long listLength = await db.ListRightPushAsync(idKey, featureValues.ToArray());
            await db.SetAddAsync(GetLabelledIndexKey(), id);
        }

        public override async Task AddLabels(IDictionary<string, string> idLabelLookups)
        {
            var db = RedisDatabase.GetDatabase();

            foreach (var kvp in idLabelLookups)
            {
                string id = kvp.Key;
                string label = kvp.Value;
                RedisKey idKey = GetRedisID(id);
                await db.ListRightPushAsync(idKey, label);
                await db.SetRemoveAsync(GetUnlabelledIndexKey(), id);
                await db.SetAddAsync(GetLabelledIndexKey(), id);
            }
        }

        public override async Task AddUnlabelledRow(object[] unlabelled)
        {
            string id = unlabelled[0].ToString();
            RedisKey idKey = GetRedisID(id);
            IEnumerable<RedisValue> featureValues = unlabelled.Select(d => (RedisValue)(d.ToString()));

            var db = RedisDatabase.GetDatabase();
            long listLength = await db.ListRightPushAsync(idKey, featureValues.ToArray());
            await db.SetAddAsync(GetUnlabelledIndexKey(), id);
        }

        public override void Clear()
        {
            RedisDatabase.DeleteDatabase();
        }

        protected override async Task<object[]> GetItemById(string id)
        {
            var redisId = GetRedisID(id);
            var db = RedisDatabase.GetDatabase();
            return ConvertToTypedRow(await db.ListRangeAsync(redisId));
        }

        protected override Task<IEnumerable<object[]>> GetRawLabelledData()
        {
            return GetRowsFromIndex(GetLabelledIndexKey());
        }

        protected override Task<IEnumerable<object[]>> GetRawUnlabelledData()
        {
            return GetRowsFromIndex(GetUnlabelledIndexKey());
        }
    }
}
