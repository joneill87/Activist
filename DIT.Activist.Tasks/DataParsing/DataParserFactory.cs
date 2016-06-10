using DIT.Activist.Domain.Interfaces.Data;
using DIT.Activist.Tasks.DataParsing.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Tasks.DataParsing
{
    public class DataParserFactory : IDataParserFactory
    {
        public IDataParser Create(DataFormats outputFormat)
        {
            switch (outputFormat)
            {
                case DataFormats.CIFAR10:
                    return new CIFAR10Parser();
                case DataFormats.MNIST:
                    throw new NotImplementedException("MNIST Parser not implemented");
                default:
                    throw new Exception("Unrecognized data format " + outputFormat.ToString());
            }

        }
    }
}
