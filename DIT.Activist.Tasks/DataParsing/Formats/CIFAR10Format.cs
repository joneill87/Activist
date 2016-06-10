using DIT.Activist.Domain.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Tasks.DataParsing.Formats
{
    internal class CIFAR10Format : IDataFormat
    {
        internal const int CIFAR_IMAGE_WIDTH = 32;
        internal const int CIFAR_IMAGE_HEIGHT = 32;
        internal const int CIFAR_IMAGE_CHANNEL_SIZE = 1024;
        internal const int CIFAR_IMAGE_NUM_BYTES = (CIFAR_IMAGE_CHANNEL_SIZE * 3) + 1;

        private const int idIndex = 0;
        private const int featureStartIndex = 1;
        private const int featureEndIndex = CIFAR_IMAGE_NUM_BYTES - 1;
        private const int featureCount = featureEndIndex - featureStartIndex + 1;
        private const int artifactIndex = featureEndIndex + 1;
        private const int labelIndex = artifactIndex + 1;
        private const int arrLength = labelIndex + 1;

        private readonly Type featureType = typeof(double);
        private readonly Type labelType = typeof(int);

        public int ArrayLength
        {
            get
            {
                return arrLength;
            }
        }

        public Type FeatureType
        {
            get
            {
                return featureType;
            }
        }

        public Type LabelType
        {
            get
            {
                return labelType;
            }
        }

        private bool IsValidFeatureType(Type typeRequested)
        {
            return typeRequested.IsAssignableFrom(featureType);
        }

        private bool IsValidLabelType(Type typeRequested)
        {
            return typeRequested.IsAssignableFrom(labelType);
        }

        public long GetID(object[] row)
        {
            return Convert.ToInt64(row[idIndex]);
        }

        public string GetArtifact(object[] row)
        {
            return row[artifactIndex].ToString();
        }

        public T GetLabel<T>(object[] row)
        {
            if (IsValidLabelType(typeof(T)))
            {
                return (T)row[labelIndex];
            }
            else
            {
                throw new InvalidCastException(String.Format("CIFAR-10 Feature type is int32, cannot assign to {0} from int32", typeof(T).Name));
            }
        }

        public IEnumerable<T> GetFeatures<T>(object[] row)
        {
            Type requestedType = typeof(T);
            if (IsValidFeatureType(requestedType))
            {
                if (requestedType == typeof(object))
                {
                    return (IEnumerable<T>)(new ArraySegment<object>(row, featureStartIndex, featureCount).AsEnumerable());
                }
                else
                {
                    return new ArraySegment<object>(row, featureStartIndex, featureCount).Select(v => (T)System.Convert.ChangeType(v, typeof(T)));
                }
            }
            else
            {
                throw new InvalidCastException(String.Format("CIFAR-10 Feature type is double, cannot assign to {0} from double", typeof(T).Name));
            }
        }

        public object[] CreateEmptyRow()
        {
            return new object[arrLength];
        }

        public void SetID(object[] row, long id)
        {
            row[idIndex] = id;
        }

        public void SetArtifact(object[] row, string value)
        {
            row[artifactIndex] = value;
        }

        public void SetLabel<T>(object[] row, T value)
        {
            row[labelIndex] = value;
        }

        public void SetFeatures<T>(object[] row, IEnumerable<T> values)
        {
            Array.Copy(values.ToArray(), 0, row, featureStartIndex, featureCount);
        }
    }
}
