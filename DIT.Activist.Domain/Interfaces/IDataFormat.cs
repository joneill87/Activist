using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Domain.Interfaces
{
    public interface IDataFormat
    {
        int ArrayLength { get; }
        int ArtifactIndex { get; }
        int FeatureIndexEnd { get; }
        int FeatureIndexStart { get; }
        int IdIndex { get; }
        int LabelIndex { get; }
        int FeatureCount { get; }
    }
}
