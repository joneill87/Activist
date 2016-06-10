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
        private Random randomGenerator;

        public RandomSeedingStrategy()
        {
            
        }

        public IEnumerable<string> ParameterNames { get { return new string[] { "randomSeed" }; } }

        public void Initialize(Dictionary<string, string> parameters)
        {
            int randomSeed = Convert.ToInt32(parameters["randomSeed"]);
            randomGenerator = new Random(randomSeed);
        }
        public async Task<IEnumerable<long>> GetQueryIDs(IDataStore dataStore, IDataFormat format, int seedSize)
        {
            var unlabelled = (await dataStore.GetUnlabelled()).ToList();
            int unlabelledCount = unlabelled.Count;
            int numRequired = Math.Min(unlabelledCount, seedSize);
            List<long> queriedIds = new List<long>(numRequired);

            while (queriedIds.Count < numRequired)
            {
                object[] row = unlabelled[randomGenerator.Next(unlabelledCount)];
                long id = format.GetID(row);
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
