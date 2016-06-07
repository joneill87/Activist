using DIT.Activist.Domain.Interfaces;
using DIT.Activist.Domain.Models;
using DIT.Activist.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Tasks
{
    public static class ActiveLoop
    {
        public static async Task<JobIteration> GetSeedIteration(LabellingJob labellingJob, IJobIterationRepository iterationRepo, ISeedingStrategy seedingStrategy)
        {
            IDataStore ds = labellingJob.DataStore;
            JobIteration iteration = new JobIteration()
            {
                JobID=labellingJob.JobID,
                PreviousIterationID=Guid.Empty,
                QueryIDs=(await seedingStrategy.GetQueryIDs(ds, 3)).ToArray()
            };

            await iterationRepo.Add(iteration);
            return iteration;
        }

        public static async Task<JobIteration> DoIteration(IJobIterationNotifier jobIterationNotifier, ILabellingJobRepository jobRepo, IJobIterationRepository iterationRepo, Guid jobIterationId, string[] labels)
        {
            //add the newly supplied labels to the previous jobIteration object
            JobIteration iteration = iterationRepo.Get(jobIterationId);
            iteration.QueryLabels = labels;
            await iterationRepo.Update(iteration);

            //now update the datastore with the supplied labels
            Guid labellingJobId = iteration.JobID;
            LabellingJob job = await jobRepo.Get(labellingJobId);
            IDataStore dataStore = job.DataStore;
            var idLabelLookups = iteration.QueryIDs.Zip(labels, (key, value) => new { key, value })
                .ToDictionary(item => item.key, item => item.value);

            await dataStore.AddLabels(idLabelLookups);


            JobIteration newIteration = new JobIteration()
            {
                IterationID = Guid.NewGuid(),
                JobID = labellingJobId,
                PreviousIterationID = jobIterationId
            };

            IEnumerable<object[]> unlabelled = await dataStore.GetUnlabelled();
            IEnumerable<object[]> labelled = await dataStore.GetLabelled();

            //check to see if we should stop the active labelling process
            IStoppingCriterion stoppingCriterion = job.StoppingCriterion;
            bool shouldStop = await stoppingCriterion.ShouldStop(unlabelled, labelled);

            if (shouldStop == false)
            {
                //we should keep going so we need to solicit a new batch of queries from the selection strategy
                ISelectionStrategy selectionStrategy = job.SelectionStrategy;

                
                int batchSize = job.BatchSize;


                var idQueries = await selectionStrategy.GenerateQuery(labelled, unlabelled, batchSize);
                newIteration.QueryIDs = idQueries.ToArray();
            }

            //save the new iteration
            await iterationRepo.Add(newIteration);

            if (shouldStop)
            {
                jobIterationNotifier.LabellingJobComplete();
            }
            else
            {
                jobIterationNotifier.OnLabelsRequested(newIteration);
            }

            return newIteration;
        }

        public static async Task<LabellingJob> CreateLabellingJob(ILabellingJobRepository jobRepo, 
            IJobIterationRepository iterationRepo, IDataStore dataStore, IPredictiveModel model, 
            ISelectionStrategy selectionStrategy, IStoppingCriterion stoppingCriterion, 
            ISeedingStrategy seedingStrategy, IJobIterationNotifier notifier, 
            int batchSize, int seedSize)
        {
            var job = new LabellingJob()
            {
                JobID=Guid.NewGuid(),
                DataStore = dataStore,
                Model = model,
                SelectionStrategy = selectionStrategy,
                StoppingCriterion = stoppingCriterion,
                BatchSize = batchSize
            };

            await jobRepo.Add(job);

            JobIteration iteration = new JobIteration()
            {
                JobID = job.JobID,
                PreviousIterationID = Guid.Empty,
                QueryIDs = (await seedingStrategy.GetQueryIDs(dataStore, seedSize)).ToArray()
            };

            await iterationRepo.Add(iteration);

            notifier.OnLabelsRequested(iteration);

            return job;
        }
    }
}
