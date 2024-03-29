﻿using DIT.Activist.ActiveLearning.Factories;
using DIT.Activist.Domain.Interfaces;
using DIT.Activist.Domain.Interfaces.Data;
using DIT.Activist.Domain.Interfaces.Factories;
using DIT.Activist.Domain.Models;
using DIT.Activist.Domain.Interfaces.Repositories;
using DIT.Activist.Infrastructure;
using DIT.Activist.Infrastructure.Factories;
using DIT.Activist.Repositories.MemCache;
using DIT.Activist.Tasks;
using DIT.Activist.Tasks.DataParsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIT.Activist.Repositories;
using DIT.Activist.Domain.Interfaces.ActiveLoop;

namespace AdHocTesting
{
    public static class AdHocTesting
    {
        public static async Task<LabellingJob> RunCreateJobTest()
        {
            ILabellingJobRepository jobRepo = new LabellingJobRepositoryFactory().Create();
            IJobIterationRepository iterationRepo = new JobIterationRepositoryFactory().Create();

            IDataParser parser = new DataParserFactory().Create(DataFormats.CIFAR10);
            IDataFormat dataFormat = parser.Format;
            IDataStore dataStore = new DataStoreFactory().CreateOrConnect("TestJack", dataFormat);
            
            IPredictiveModelFactory modelFactory = new PredictiveModelFactory();
            IPredictiveModel model = modelFactory.Create("LinearRegression");

            ISelectionStrategyFactory ssFactory = new SelectionStrategyFactory();
            Dictionary<string, string> ssParameters = new Dictionary<string, string>();
            ssParameters.Add("randomSeed", "15");
            ISelectionStrategy selectionStrategy = ssFactory.Create("RandomSelectionStrategy", ssParameters);

            IStoppingCriterionFactory scFactory = new StoppingCriterionFactory();
            Dictionary<string, string> scParameters = new Dictionary<string, string>();
            scParameters.Add("maxLabels", "15");
            IStoppingCriterion stoppingCriterion = scFactory.Create("LabelLimit", scParameters);

            ISeedingStrategyFactory seedingFactory = new SeedingStrategyFactory();
            Dictionary<string, string> seedingParameters = new Dictionary<string, string>();
            seedingParameters.Add("randomSeed", "15");
            ISeedingStrategy seedingStrategy = seedingFactory.Create("RandomSeedingStrategy", seedingParameters);

            IJobIterationNotifier notifier = new JobIterationNotifierFactory().Create();

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
                dataFormat: dataFormat,
                batchSize: batchSize,
                seedSize: seedSize);

            return job;
        }

        public static async Task RunIterationTest(Guid iterationId, string[] labels)
        {
            ILabellingJobRepository jobRepo = new LabellingJobRepositoryFactory().Create();
            IJobIterationNotifier notifier = new JobIterationNotifierFactory().Create();
            IJobIterationRepository iterationRepo = new JobIterationRepositoryFactory().Create();

            JobIteration ji = iterationRepo.Get(iterationId);
            Guid jobId = ji.JobID;

            await ActiveLoop.DoIteration(notifier, jobRepo, iterationRepo, iterationId, labels);
        }
    }
}
