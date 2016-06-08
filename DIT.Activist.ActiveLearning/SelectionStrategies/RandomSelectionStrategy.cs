﻿using DIT.Activist.Domain.Interfaces;
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
