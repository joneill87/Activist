using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Domain.Interfaces.Data
{
    public interface IDataParser
    {
        IDataFormat Format { get; }

        IEnumerable<object[]> ExtractFeatureValues(Stream byteStream, int limit = -1);

        IEnumerable<KeyValuePair<object, object>> ExtractLabels(Stream byteStream);

        IEnumerable<object[]> ExtractFeaturesAndLabels(Stream byteStream, int limit = -1);
    }
}
