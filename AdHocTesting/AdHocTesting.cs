using DIT.Activist.ActiveLearning.Models;
using DIT.Activist.ActiveLearning.SeedingStrategies;
using DIT.Activist.ActiveLearning.SelectionStrategies;
using DIT.Activist.ActiveLearning.StoppingCriteria;
using DIT.Activist.Domain.Interfaces;
using DIT.Activist.Domain.Models;
using DIT.Activist.Domain.Repositories;
using DIT.Activist.Infrastructure;
using DIT.Activist.Infrastructure.Datastores.InMemory;
using DIT.Activist.Repositories.MemCache;
using DIT.Activist.Tasks;
using DIT.Activist.Tasks.DataParsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdHocTesting
{
    public static class AdHocTesting
    {
        public static async Task<LabellingJob> RunCreateJobTest()
        {
            ILabellingJobRepository jobRepo = new MCLabellingJobRepository();
            IJobIterationRepository iterationRepo = new MCJobIterationRepository();
            IDataStore dataStore = new MemCacheDataStore(CIFAR10Parser.Format);
            IPredictiveModel model = new LinearRegression();
            ISelectionStrategy selectionStrategy = new RandomSelectionStrategy(15);
            IStoppingCriterion stoppingCriterion = new LabelLimit(15);
            ISeedingStrategy seedingStrategy = new RandomSeedingStrategy(15);
            IJobIterationNotifier notifier = JobIterationNotifier.Instance;

            int batchSize = 3;
            int seedSize = 3;

            var job = await ActiveLoop.CreateLabellingJob(
                jobRepo: jobRepo, 
                iterationRepo: iterationRepo, 
                dataStore: dataStore, 
                model: model, 
                selectionStrategy: selectionStrategy, 
                stoppingCriterion: stoppingCriterion, 
                seedingStrategy: seedingStrategy, 
                notifier: notifier,
                batchSize: batchSize,
                seedSize: seedSize);

            return job;
        }

        public static async Task RunIterationTest(Guid iterationId, string[] labels)
        {
            ILabellingJobRepository jobRepo = new MCLabellingJobRepository();
            IJobIterationNotifier notifier = JobIterationNotifier.Instance;
            IJobIterationRepository iterationRepo = new MCJobIterationRepository();

            JobIteration ji = iterationRepo.Get(iterationId);
            Guid jobId = ji.JobID;

            await ActiveLoop.DoIteration(notifier, jobRepo, iterationRepo, iterationId, labels);
        }
    }
}
