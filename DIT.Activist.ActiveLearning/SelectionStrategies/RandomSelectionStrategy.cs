using DIT.Activist.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.ActiveLearning.SelectionStrategies
{
    public class RandomSelectionStrategy : ISelectionStrategy
    {
        private Random randomGenerator;

        public RandomSelectionStrategy(int seed)
        {
            randomGenerator = new Random(seed);
        }

        public Task<ICollection<string>> GenerateQuery(IEnumerable<object[]> labelled, IEnumerable<object[]> unlabelled, int batchSize)
        {
            List<object[]> unlabelledList = unlabelled.ToList();

            int idsRequired = Math.Min(unlabelledList.Count, batchSize);
            List<string> queryIds = new List<string>(idsRequired);

            for (int ii=0; ii<idsRequired; ii++)
            {
                int randomIndex = randomGenerator.Next(unlabelledList.Count);
                queryIds.Add(unlabelledList[randomIndex][0].ToString());
                unlabelledList.RemoveAt(randomIndex);
            }

            return Task.FromResult<ICollection<string>>(queryIds);
        }
    }
}
