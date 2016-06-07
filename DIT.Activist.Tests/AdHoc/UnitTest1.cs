using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DIT.Activist.Tasks.DataParsing;
using DIT.Activist.Infrastructure;
using System.IO;
using DIT.Activist.Domain.Interfaces;
using System.Collections.Generic;
using DIT.Activist.ActiveLearning.Models;
using System.Threading.Tasks;
using DIT.Activist.Infrastructure.Datastores.InMemory;

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

        [TestMethod]
        public void KNN_Does_Its_Stuff()
        {
            const int TRAIN_LIMIT = 1000;
            const int TEST_LIMIT = 500;
            var parser = new CIFAR10Parser();
            IEnumerable<object[]> dataInput = parser.ExtractFeaturesAndLabels(File.Open(Path.Combine(basePath, "dataBatchFull.bin"), FileMode.Open), TRAIN_LIMIT);
            IDataStore dataStore = new DataStoreFactory().Create(CIFAR10Parser.Format);
            dataStore.Clear();
            var addTask = dataStore.AddLabelledRow(dataInput);
            Task.WaitAll(addTask);
            IPredictiveModel model = new KNearestNeighbour(5, 10);

            var input = dataStore.GetLabelled().Result.Take(TRAIN_LIMIT).ToArray();
            input = input.Select(r => r.Take(r.Length - 1).ToArray()).ToArray();
            var ids = Enumerable.Range(1, TRAIN_LIMIT).Select(i => i.ToString());
            var labels = dataStore.GetLabelById(ids).Result.Take(TRAIN_LIMIT).ToArray();
            model.Train(input.Select(r => r.Select(c => System.Convert.ToDouble(c)).ToArray()), labels.Select(l => System.Convert.ToDouble(l)));

            IEnumerable<object[]> test = parser.ExtractFeatureValues(File.Open(Path.Combine(basePath, "dataBatch5.bin"), FileMode.Open)).Take(TEST_LIMIT);
            Dictionary<object, object> testLabels = parser.ExtractLabels(File.Open(Path.Combine(basePath, "dataBatch5_2.bin"), FileMode.Open)).Take(TEST_LIMIT).ToDictionary(k => k.Key, v => v.Value);

            int correct = 0;
            int incorrect = 0;
            object[][] testArr = test.ToArray();
            foreach (object[] row in testArr)
            {
                var prediction = model.Compute(row.Skip(1).Take(row.Length - 2).Select(r => System.Convert.ToDouble(r)).ToArray());
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
    }
}
