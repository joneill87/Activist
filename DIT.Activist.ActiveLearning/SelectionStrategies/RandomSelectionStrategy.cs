using DIT.Activist.Domain.Interfaces;
using DIT.Activist.Domain.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.ActiveLearning.SelectionStrategies
{
    internal class RandomSelectionStrategy : ISelectionStrategy
    {
        private Random randomGenerator;

        public RandomSelectionStrategy()
        {
            
        }

        public void Initialize(Dictionary<string, string> parameters)
        {
            int randomSeed = Convert.ToInt32(parameters["randomSeed"]);
            randomGenerator = new Random(randomSeed);
        }

        public IEnumerable<string> ParameterNames { get { return new string[] { "randomSeed" }; } }

        public Task<ICollection<long>> GenerateQuery(IEnumerable<object[]> labelled, IEnumerable<object[]> unlabelled, IDataFormat dataFormat, int batchSize)
        {
            List<object[]> unlabelledList = unlabelled.ToList();

            int idsRequired = Math.Min(unlabelledList.Count, batchSize);
            List<long> queryIds = new List<long>(idsRequired);

            for (int ii=0; ii<idsRequired; ii++)
            {
                int randomIndex = randomGenerator.Next(unlabelledList.Count);
                var randomRow = unlabelledList[randomIndex];
                queryIds.Add(dataFormat.GetID(randomRow));
                unlabelledList.RemoveAt(randomIndex);
            }

            return Task.FromResult<ICollection<long>>(queryIds);
        }
    }
}
