using DIT.Activist.Domain.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DIT.Activist.Tasks.DataParsing.Formats;

namespace DIT.Activist.Tasks.DataParsing.Parsers
{
    public class MNistParser : IDataParser
    {
        private MNistFormat format = new MNistFormat();

        public IDataFormat Format
        {
            get
            {
                return format;
            }
        }

        public IEnumerable<object[]> ExtractFeaturesAndLabels(Stream byteStream, int limit = -1)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object[]> ExtractFeatureValues(Stream byteStream, int limit = -1)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<KeyValuePair<object, object>> ExtractLabels(Stream byteStream)
        {
            throw new NotImplementedException();
        }
    }
}
