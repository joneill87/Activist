using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DIT.Activist.Tasks.DataParsing;
using DIT.Activist.Infrastructure;
using System.IO;
using DIT.Activist.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Accord.Math;
using DIT.Activist.Infrastructure.Factories;
using DIT.Activist.Domain.Interfaces.Factories;
using DIT.Activist.Domain.Interfaces.ActiveLearning;
using DIT.Activist.ActiveLearning.Factories;
using DIT.Activist.Domain.Interfaces.Data;
using DIT.Activist.Tasks.DataParsing.Parsers;

namespace DIT.Activist.Tests.AdHoc
{
    [TestClass]
    public class UnitTest1
    {
        string basePath = @"C:\Users\Jack\Downloads\cifar10";
        string[] fileNames = new string[]
        {
                "dataBatch1.bin",
                "dataBatch2.bin",
                "dataBatch3.bin",
                "dataBatch4.bin"
        };

        private CombinationStream GetComboStream()
        {
            return new CombinationStream(fileNames.Select(f => File.OpenRead(Path.Combine(basePath, f))));
        }

        [TestMethod]
        public void Jacks_Combo_Stream_Is_Amazing()
        {
            //using (var comboStream = new CombinationStream(fileNames.Select(f => File.OpenRead(Path.Combine(basePath, f)))))
            //{
            //    using (var reader = new StreamReader(comboStream))
            //    {
            //        reader.ReadToEnd();
            //    }
            //}
            
        }
        //[TestMethod]
        public void KNN_Does_Its_Stuff()
        {
            const int TRAIN_LIMIT = 50;
            const int TEST_LIMIT = 500;
            var parser = new CIFAR10Parser();
            IDataFormat dataFormat = parser.Format;

            //gather the data
            IEnumerable<object[]> dataInput = parser.ExtractFeaturesAndLabels(File.Open(Path.Combine(basePath, "dataBatchFull.bin"), FileMode.Open), TRAIN_LIMIT);

            //clear and repopulate the datastore
            IDataStore dataStore = new DataStoreFactory().CreateOrReplace("TestJack", dataFormat);
            dataStore.Clear();
            var addTask = dataStore.AddLabelledRow(dataInput);
            Task.WaitAll(addTask);

            IPredictiveModelFactory modelFactory = new PredictiveModelFactory();
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            parameters.Add("k", "5");
            parameters.Add("numberOfClasses", "10");
            IPredictiveModel model = modelFactory.Create("KNearestNeighbour", parameters);

            object[][] data = dataStore.GetLabelled().Result.Take(TRAIN_LIMIT).ToArray();
            var features = data.GetFeatures<double>(dataFormat);
            
            //we know our ids are sequential from 1 so generate a list of ids for ourselves
            var ids = Enumerable.Range(1, TRAIN_LIMIT).Select(i => (long)i);
            var labels = dataStore.GetLabelById(ids).Result.Take(TRAIN_LIMIT).Select(l => Convert.ToDouble(l));

            model.Train(features, labels);

            var testData = parser.ExtractFeatureValues(File.Open(Path.Combine(basePath, "dataBatch5.bin"), FileMode.Open)).Take(TEST_LIMIT);
            var testLabels = parser.ExtractLabels(File.Open(Path.Combine(basePath, "dataBatch5_2.bin"), FileMode.Open)).Take(TEST_LIMIT).ToDictionary(k => k.Key, v => v.Value);

            int correct = 0;
            int incorrect = 0;
            object[][] testArr = testData.ToArray();
            foreach (object[] row in testArr)
            {
                var prediction = model.Compute(dataFormat.GetFeatures<double>(row).ToArray());
                if (System.Convert.ToInt32(prediction) == System.Convert.ToInt32(testLabels[System.Convert.ToInt32(row[0])]))
                {
                    correct++;
                }
                else
                {
                    incorrect++;
                }
            }

            Assert.IsTrue(correct > 0);
        }

        private class TestDataFormat : IDataFormat
        {
            public int ArrayLength { get { return 4; } }

            public Type FeatureType { get { return typeof(double); } }

            public Type LabelType { get { return typeof(double); } }

            public object[] CreateEmptyRow()
            {
                return new object[4];
            }

            public string GetArtifact(object[] row)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<T> GetFeatures<T>(object[] row)
            {
                return row.Skip(1).Select(r => (T)Convert.ChangeType(r, typeof(T))).ToArray();
            }

            public long GetID(object[] row)
            {
                return Convert.ToInt64(row[0]);
            }

            public T GetLabel<T>(object[] row)
            {
                throw new NotImplementedException();
            }

            public void SetArtifact(object[] row, string value)
            {
                throw new NotImplementedException();
            }

            public void SetFeatures<T>(object[] row, IEnumerable<T> values)
            {
                throw new NotImplementedException();
            }

            public void SetID(object[] row, long id)
            {
                throw new NotImplementedException();
            }

            public void SetLabel<T>(object[] row, T value)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void Diversity_SS_Is_Awesome()
        {
            ISelectionStrategyFactory ssFactory = new SelectionStrategyFactory();
            Dictionary<string, string> ssParameters = new Dictionary<string, string>();
            ssParameters.Add("diversityFunction", DiversityFunctions.COSINE_DISTANCE.ToString());
            ISelectionStrategy diversity = ssFactory.Create("DiversitySelectionStrategy", ssParameters);

            object[][] labelled = new object[][]
            {
                new object[] {1, 5, 5, 5}
            };
            object[][] unlabelled = new object[][]
            {
                new object[] {2, 6, 5, 6},
                new object[] {3, 8, 18, 218}
            };

            var selected = diversity.GenerateQuery(labelled, unlabelled, new TestDataFormat(), 1).Result;
            Assert.AreEqual(selected.Count, 1);
            Assert.AreEqual(selected.First(), 3);
        }
    }
}
