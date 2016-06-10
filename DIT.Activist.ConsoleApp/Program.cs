using DIT.Activist.ActiveLearning.Factories;
using DIT.Activist.ActiveLearning.StoppingCriteria;
using DIT.Activist.Domain.Interfaces;
using DIT.Activist.Domain.Interfaces.ActiveLearning;
using DIT.Activist.Domain.Interfaces.Factories;
using DIT.Activist.Domain.Repositories;
using DIT.Activist.Infrastructure;
using DIT.Activist.Infrastructure.Factories;
using DIT.Activist.Repositories.MemCache;
using DIT.Activist.Tasks.DataParsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ILabellingJobRepository jobRepo = new MCLabellingJobRepository();
            IJobIterationRepository iterationRepo = new MCJobIterationRepository();

            IDataStore dataStore = new DataStoreFactory().Create(CIFAR10Parser.Format);

            IPredictiveModelFactory modelFactory = new PredictiveModelFactory();
            Dictionary<string, string> modelParameters = new Dictionary<string, string>();
            modelParameters.Add("k", "15");
            modelParameters.Add("numberOfClasses", "10");
            IPredictiveModel model = modelFactory.Create("KNearestNeighbour", modelParameters);

            ISelectionStrategyFactory ssFactory = new SelectionStrategyFactory();
            Dictionary<string, string> ssParameters = new Dictionary<string, string>();
            ssParameters.Add("diversityFunction", DiversityFunctions.COSINE_DISTANCE.ToString());
            ISelectionStrategy selectionStrategy = ssFactory.Create("DiversitySelectionStrategy", ssParameters);

            IStoppingCriterionFactory scFactory = new StoppingCriterionFactory();
            Dictionary<string, string> scParameters = new Dictionary<string, string>();
            scParameters.Add("maxLabels", "15");
            IStoppingCriterion stoppingCriterion = scFactory.Create("LabelLimit", scParameters);

            ISeedingStrategyFactory seedingFactory = new SeedingStrategyFactory();
            Dictionary<string, string> seedingParameters = new Dictionary<string, string>();
            seedingParameters.Add("randomSeed", "15");
            ISeedingStrategy seedingStrategy = seedingFactory.Create("RandomSeedingStrategy", seedingParameters);

            IJobIterationNotifier notifier = JobIterationNotifier.Instance;
            IDataFormat dataFormat = CIFAR10Parser.Format;
        }

        static void Test(Func<double[], double[], double> func)
        {
            
        }
    }
}
