using Accord.Math;
using DIT.Activist.Domain.Interfaces;
using DIT.Activist.Domain.Interfaces.ActiveLearning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.ActiveLearning.SelectionStrategies
{
    public class DiversitySelectionStrategy : ISelectionStrategy
    {
        private Func<object[], object[], double> diversityFunction;

        public DiversitySelectionStrategy()
        {
           
        }

        public void Initialize(Dictionary<string, string> parameters)
        {
            DiversityFunctions diversityFunc = (DiversityFunctions)Enum.Parse(typeof(DiversityFunctions), parameters["diversityFunction"]);
            switch (diversityFunc)
            {
                case DiversityFunctions.COSINE_DISTANCE:
                    diversityFunction = (object[] x, object[] y) =>
                    {
                        double[] xArr = x.Select(i => Convert.ToDouble(i)).ToArray();
                        double[] yArr = y.Select(i => Convert.ToDouble(i)).ToArray();
                        return Distance.Cosine(xArr, yArr);
                    };
                    break;
                default:
                    throw new ArgumentException("Unrecognized diversity function");
            }
        }

        public IEnumerable<string> ParameterNames { get { return new string[] { "diversityFunction" }; } }

        public Task<ICollection<long>> GenerateQuery(IEnumerable<object[]> labelled, IEnumerable<object[]> unlabelled, IDataFormat dataFormat, int batchSize)
        {
            DiversityComparer diversityComparer = new DiversityComparer(labelled.GetFeatures<object>(dataFormat), diversityFunction);
            return Task.FromResult<ICollection<long>>(unlabelled.OrderByDescending(u => 
                    diversityComparer.GetMinimumDistance(dataFormat.GetFeatures<object>(u)))
                .Take(batchSize)
                .Select(u => dataFormat.GetID(u))
                .ToList());
        }

        private class DiversityComparer
        {
            private object[][] labelled;
            Func<object[], object[], double> diversityFunction;

            public DiversityComparer(IEnumerable<object[]> labelled, Func<object[], object[], double> diversityFunction)
            {
                this.labelled = labelled.ToArray();
                this.diversityFunction = diversityFunction;
            }

            public double GetMinimumDistance(IEnumerable<object> source)
            {

                var minDistance = labelled.Select(o => diversityFunction(source.ToArray(), o)).Min();
                return minDistance;

            }
        }
    }
}
