using DIT.Activist.Domain;
using DIT.Activist.Domain.Models;
using DIT.Activist.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Repositories.MemCache
{
    public class MCLabellingJobRepository : ILabellingJobRepository
    {
        private static readonly Dictionary<Guid, LabellingJob> cache = new Dictionary<Guid, LabellingJob>();

        public Task Add(LabellingJob job)
        {
            if (job.JobID == Guid.Empty)
            {
                job.JobID = Guid.NewGuid();
            }

            cache.Add(job.JobID, job);
            return Task.FromResult<object>(null);
        }

        public Task<LabellingJob> Get(Guid labellingJobId)
        {
            LabellingJob result;

            if (cache.Keys.Contains(labellingJobId))
            {
                result = cache[labellingJobId];
            }
            else
            {
                result = null;
            }

            return Task.FromResult(result);
        }
    }
}
