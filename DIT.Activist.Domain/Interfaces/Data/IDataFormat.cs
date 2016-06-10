using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Domain.Interfaces.Data
{
    public interface IDataFormat
    {
        
        int ArrayLength { get; }

        Type FeatureType { get; }

        Type LabelType { get; }

        object[] CreateEmptyRow();

        long GetID(object[] row);

        void SetID(object[] row, long id);

        string GetArtifact(object[] row);

        void SetArtifact(object[] row, string value);

        T GetLabel<T>(object[] row);

        void SetLabel<T>(object[] row, T value);

        IEnumerable<T> GetFeatures<T>(object[] row);

        void SetFeatures<T>(object[] row, IEnumerable<T> values);
    }

    public static class IDataFormatExtensionMethods
    {
        public static IEnumerable<long> GetIDs(this object[][] dataset, IDataFormat format)
        {
            return dataset.Select(d => format.GetID(d));
        }

        public static IEnumerable<T> GetLabels<T>(this object[][] dataset, IDataFormat format)
        {
            return dataset.Select(d => format.GetLabel<T>(d));
        }

        public static IEnumerable<string> GetArtifacts(this object[][] dataset, IDataFormat format)
        {
            return dataset.Select(d => format.GetArtifact(d));
        }

        public static IEnumerable<T[]> GetFeatures<T>(this IEnumerable<object[]> dataset, IDataFormat format)
        {
            return dataset.Select(d => format.GetFeatures<T>(d).ToArray());
        }

        public static IEnumerable<T[]> AsArrays<T>(this IEnumerable<IEnumerable<T>> dataset)
        {
            return dataset.Select(d => d.ToArray());
        }
    }
}
