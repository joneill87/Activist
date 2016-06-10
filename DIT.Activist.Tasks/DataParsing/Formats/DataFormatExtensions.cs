using DIT.Activist.Domain.Interfaces.Data;
using DIT.Activist.Tasks.DataParsing.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Tasks.DataParsing.Formats
{
    public static class DataFormatExtensions
    {
        public static IDataFormat GetFormat(this DataFormats formats)
        {
            switch (formats)
            {
                case DataFormats.CIFAR10:
                    return new CIFAR10Parser().Format;
                case DataFormats.MNIST:
                    throw new NotImplementedException("MNIST Data format not implemented");
                default:
                    throw new Exception("Unrecognized data format");
            }
        }
    }
}
