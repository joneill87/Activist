using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.MachineLearning;
using AccordKNN = Accord.MachineLearning.KNearestNeighbors<double[]>;
using DIT.Activist.Domain.Interfaces;
using Accord.Math;

namespace DIT.Activist.ActiveLearning.Models
{
    internal class KNearestNeighbour : IPredictiveModel
    {
        private AccordKNN knn;
        private int k;
        private int numberOfClasses;

        public KNearestNeighbour()
        {
            
        }

        public void Initialize(Dictionary<string, string> parameters)
        {
            this.k = Convert.ToInt32(parameters["k"]);
            this.numberOfClasses = Convert.ToInt32(parameters["numberOfClasses"]);
        }

        public IEnumerable<string> ParameterNames
        {
            get { return new string[] { "k", "numberOfClasses" }; }
        }

        public double[] Compute(double[][] inputs)
        {
            return inputs.Select(i => System.Convert.ToDouble(knn.Compute(i))).ToArray();
        }

        public double Compute(double[] input)
        {
            return knn.Compute(input);
        }

        public void Train(IEnumerable<double[]> inputs, IEnumerable<double> labels)
        {
            this.knn = new AccordKNN(k, inputs.ToArray(), labels.Select(l => System.Convert.ToInt32(l)).ToArray(), Distance.Cosine);
        }
    }
}
