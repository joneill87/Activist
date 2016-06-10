using DIT.Activist.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Domain.Interfaces.Repositories
{
    public interface IJobIterationRepository
    {
        JobIteration Get(Guid iterationId);
        Task Add(JobIteration newIteration);
        Task Update(JobIteration iteration);
    }
}
