using DIT.Activist.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.ActiveLearning.SeedingStrategies
{
    public class RandomSeedingStrategy : ISeedingStrategy
    {
        private readonly Random randomGenerator;
        public RandomSeedingStrategy(int randomSeed)
        {
            randomGenerator = new Random(randomSeed);
        }

        public async Task<IEnumerable<string>> GetQueryIDs(IDataStore dataStore, int seedSize)
        {
            var unlabelled = (await dataStore.GetUnlabelled()).ToList();
            int unlabelledCount = unlabelled.Count;
            int numRequired = Math.Min(unlabelledCount, seedSize);
            List<string> queriedIds = new List<string>(numRequired);

            while (queriedIds.Count < numRequired)
            {
                object[] row = unlabelled[randomGenerator.Next(unlabelledCount)];
                string id = row[0].ToString();
                if (queriedIds.Contains(id) == false)
                {
                    queriedIds.Add(id);
                    unlabelledCount--;
                    unlabelled.Remove(row);
                }
            }

            return queriedIds;
        }
    }
}
