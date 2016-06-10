using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIT.Activist.Domain.Models;
using DIT.Activist.Domain.Interfaces.Repositories;

namespace DIT.Activist.Repositories.MemCache
{
    internal class MCJobIterationRepository : IJobIterationRepository
    {
        private static readonly Dictionary<Guid, JobIteration> cache = new Dictionary<Guid, JobIteration>();

        public Task Add(JobIteration newIteration)
        {
            if (newIteration.IterationID == Guid.Empty)
            {
                newIteration.IterationID = Guid.NewGuid();
            }
            cache.Add(newIteration.IterationID, newIteration);
            return Task.FromResult<object>(null);
        }

        public JobIteration Get(Guid iterationId)
        {
            if (cache.ContainsKey(iterationId))
            {
                return cache[iterationId];
            }
            else
            {
                return null;
            }
        }

        public Task Update(JobIteration iteration)
        {
            if (!cache.ContainsKey(iteration.IterationID))
            {
                throw new Exception("Iteration not found");
            }
            else
            {
                cache[iteration.IterationID] = iteration;
            }
            return Task.FromResult<object>(null);
        }
    }
}
