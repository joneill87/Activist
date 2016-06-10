using DIT.Activist.Domain.Interfaces.Data;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Infrastructure.Datastores.Redis
{
    internal static class RedisDatabase
    {
        private static bool isInitialized = false;
        private static string rootNamespace = null;
        private static ConnectionMultiplexer redis;
        private static RedisKey rootKey;
        private static RedisKey dbExistsKeySuffix = "0";

        private static RedisKey GetDatasetRootNamespace(string datasetName)
        {
            return rootKey.Append(datasetName);
        }

        private static RedisKey GetDatasetExistsKey(string datasetName)
        {
            return GetDatasetRootNamespace(datasetName).Append(dbExistsKeySuffix);
        }

        internal static IDatabase GetDatabase()
        {
            EnsureInitalized();
            return redis.GetDatabase();
        }

        public static void Initialize(string serverAddress, string rootNamespace)
        {
            if (String.IsNullOrEmpty(rootNamespace))
            {
                throw new ArgumentException("rootNamespace argument can not be null or empty");
            }
            if (rootNamespace.EndsWith("/") == false)
            {
                rootNamespace = rootNamespace + "/";
            }

            RedisDatabase.rootNamespace = rootNamespace;
            RedisDatabase.isInitialized = true;
            RedisDatabase.redis = ConnectionMultiplexer.Connect(serverAddress);

            rootKey = rootNamespace;
        }

        private static void EnsureInitalized()
        {
            if (!isInitialized)
                throw new Exception("RedisDatabase must be initialized before use");
        }

        public static RedisDataStore CreateDataset(string datasetName, IDataFormat dataFormat)
        {
            IDatabase db = GetDatabase();
            RedisKey key = GetDatasetExistsKey(datasetName);
            //this should give a description of the feature space
            db.StringSet(key, "a");

            return new RedisDataStore();
        }

        public static RedisDataStore GetDataset(string datasetName)
        {
            return null;
        }

        public static void DestroyDataset(string datasetName)
        {
            var db = GetDatabase();
            db.KeyDelete(GetDatasetExistsKey(datasetName));
        }

        public static bool DatasetExists(string datasetName)
        {
            var db = GetDatabase();
            var flag = db.StringGet(GetDatasetExistsKey(datasetName));
            return String.IsNullOrEmpty(flag) == false;
        }

        public static IEnumerable<RedisKey> GetKeys(string pattern)
        {
            var endpoints = redis.GetEndPoints(true);
            foreach (var endpoint in endpoints)
            {
                var server = redis.GetServer(endpoint);
                string keyPattern = (string)rootNamespace + "*";
                foreach (var key in server.Keys(pattern: keyPattern))
                {
                    yield return key;
                }
            }
        }

        public static void DeleteDatabase()
        {
            var db = redis.GetDatabase();
            foreach (RedisKey key in GetKeys(rootNamespace + "/"))
            {
                db.KeyDelete(key);
            }
        }
    }
}
