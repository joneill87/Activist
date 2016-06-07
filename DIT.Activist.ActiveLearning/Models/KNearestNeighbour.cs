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
    public class KNearestNeighbour : IPredictiveModel
    {
        private AccordKNN knn;
        private int k;
        private int numberOfClasses;

        public KNearestNeighbour(int k, int numberOfClasses)
        {
            this.k = k;
            this.numberOfClasses = numberOfClasses;
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
