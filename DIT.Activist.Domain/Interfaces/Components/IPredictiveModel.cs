using DIT.Activist.Domain.Interfaces.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Domain.Interfaces
{
    public interface IPredictiveModel : IActivatable
    {
        void Train(IEnumerable<double[]> inputs, IEnumerable<double> labels);
        double Compute(double[] input);
        double[] Compute(double[][] inputs);
    }
}
