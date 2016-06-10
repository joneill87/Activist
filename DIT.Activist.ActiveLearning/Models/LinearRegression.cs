using Accord.Statistics.Models.Regression.Linear;
using DIT.Activist.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.ActiveLearning.Models
{
    internal class LinearRegression : IPredictiveModel
    {
        private const bool USE_INTERCEPT = true;

        MultipleLinearRegression model;

        public LinearRegression()
        {
            
        }

        public IEnumerable<string> ParameterNames { get { return Enumerable.Empty<string>(); } }

        public void Initialize(Dictionary<string, string> parameters)
        {
            //no parameters
        }

        public void Train(IEnumerable<double[]> inputs, IEnumerable<double> labels)
        {
            if (inputs.Count() < 1)
            {
                throw new Exception("Must use at least one labelled instance to train a regression model");
            }

            int inputFeatureCount = inputs.First().Count(); //disregard the target variable for inputFeatureCount

            model = new MultipleLinearRegression(inputFeatureCount, USE_INTERCEPT);

            Error = model.Regress(inputs.ToArray(), labels.ToArray());
        }

        public double Error { get; private set; }

        public double[] Coefficients {  get { return model.Coefficients; } }

        public double Compute(double[] input)
        {
            return model.Compute(input);
        }

        public double[] Compute(double[][] inputs)
        {
            return model.Compute(inputs);
        }

        
    }
}
