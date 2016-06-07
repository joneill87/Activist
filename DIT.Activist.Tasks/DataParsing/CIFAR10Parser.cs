using DIT.Activist.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Tasks.DataParsing
{
    public class CIFAR10Parser
    {
        private const int CIFAR_IMAGE_WIDTH = 32;
        private const int CIFAR_IMAGE_HEIGHT = 32;
        private const int CIFAR_IMAGE_CHANNEL_SIZE = 1024;
        private const int CIFAR_IMAGE_NUM_BYTES = (CIFAR_IMAGE_CHANNEL_SIZE * 3) + 1;

        private const PixelFormat CIFAR_IMAGE_PIXEL_FORMAT = PixelFormat.Format32bppArgb;
        private static int imageCount = 1;

        public CIFAR10Parser() { }

        public static IDataFormat Format
        {
            get { return new CIFAR10Format(); }
        }

        public IEnumerable<object[]> ExtractFeatureValues(Stream byteStream, int limit=-1)
        {
            //the first byte is the label, skip this
            const int offset = 0;
            int bytesRead = 0;
            int imageCount = 0;

            byte[] buffer = new byte[CIFAR_IMAGE_NUM_BYTES];

            while ((bytesRead = byteStream.Read(buffer, offset, CIFAR_IMAGE_NUM_BYTES)) != 0)
            {
                //we use single element arrays here to make it easy
                //to concatenate id and feature values into 
                int[] rowId = new int[1];
                rowId[0] = ++imageCount;
                
                //the first byte is the label, skip this
                var featureValues = buffer.Skip(1).Select(b => Convert.ToInt32(b));
                string[] base64Arr = new string[1];
                base64Arr[0] = BytesToBase64Png(buffer);
                yield return rowId.Concat(featureValues).Select(i => i.ToString()).Concat(base64Arr).ToArray();

                if (limit > -1 && limit <= imageCount)
                {
                    break;
                }
            }
        }

        private string BytesToBase64Png(byte[] buffer)
        {
            IList<byte> rChannel = new ArraySegment<byte>(buffer, 1, CIFAR_IMAGE_CHANNEL_SIZE);
            IList<byte> gChannel = new ArraySegment<byte>(buffer, 1 + (CIFAR_IMAGE_CHANNEL_SIZE), CIFAR_IMAGE_CHANNEL_SIZE);
            IList<byte> bChannel = new ArraySegment<byte>(buffer, 1 + (CIFAR_IMAGE_CHANNEL_SIZE * 2), CIFAR_IMAGE_CHANNEL_SIZE);
            int label = buffer[0];

            using (Bitmap b = new Bitmap(CIFAR_IMAGE_WIDTH, CIFAR_IMAGE_HEIGHT, CIFAR_IMAGE_PIXEL_FORMAT))
            {
                int ii, jj;
                int sourceOffset = 0;
                for (ii = 0; ii < CIFAR_IMAGE_WIDTH; ii++)
                {
                    for (jj = 0; jj < CIFAR_IMAGE_WIDTH; jj++)
                    {
                        b.SetPixel(ii, jj, Color.FromArgb(255, rChannel[sourceOffset], gChannel[sourceOffset], bChannel[sourceOffset]));
                        sourceOffset++;
                    }
                }

                b.Save(@"C:\Users\Jack\Desktop\cifar\test_" + imageCount++ + "_" + label + ".png", System.Drawing.Imaging.ImageFormat.Png);

                using (MemoryStream pngBuffer = new MemoryStream())
                {
                    b.Save(pngBuffer, System.Drawing.Imaging.ImageFormat.Png);

                    return System.Convert.ToBase64String(pngBuffer.ToArray());
                }
            }
        }

        public IEnumerable<KeyValuePair<object, object>> ExtractLabels(Stream byteStream)
        {
            const int offset = 0;
            int bytesRead = 0;
            int imageCount = 0;

            byte[] buffer = new byte[CIFAR_IMAGE_NUM_BYTES];

            while ((bytesRead = byteStream.Read(buffer, offset, CIFAR_IMAGE_NUM_BYTES)) != 0)
            {
                yield return new KeyValuePair<object, object>(++imageCount, buffer[0]);
            }
        }

        public IEnumerable<object[]> ExtractFeaturesAndLabels(Stream byteStream, int limit=-1)
        {
            const int offset = 0;
            int bytesRead;
            int imageCount = 0;

            byte[] buffer = new byte[CIFAR_IMAGE_NUM_BYTES];

            while ((bytesRead = byteStream.Read(buffer, offset, CIFAR_IMAGE_NUM_BYTES)) != 0)
            {
                string[] rowId = new string[1];
                string[] label = new string[1];

                rowId[0] = (++imageCount).ToString();

                label[0] = buffer[0].ToString();
                //skip the first element in the buffer, it's the label.
                var dataRow = buffer.Skip(1).Select(b => Convert.ToInt32(b).ToString());

                string[] base64Arr = new string[1];
                base64Arr[0] = BytesToBase64Png(buffer);

                yield return rowId.Concat(dataRow).Concat(base64Arr).Concat(label).Select(i => i.ToString()).ToArray();

                if (limit > -1 && limit <= imageCount)
                {
                    break;
                }
            }
        }

        private class CIFAR10Format : IDataFormat
        {
            private const int idIndex = 0;
            private const int featureStartIndex = 1;
            private const int featureEndIndex = CIFAR_IMAGE_NUM_BYTES - 1;
            private const int artifactIndex = featureEndIndex + 1;
            private const int labelIndex = artifactIndex + 1;
            private const int arrLength = labelIndex + 1;

            public int ArrayLength { get { return arrLength; } }

            public int ArtifactIndex { get { return artifactIndex; } }

            public int FeatureIndexEnd { get { return featureEndIndex; } }

            public int FeatureIndexStart { get { return featureStartIndex; } }

            public int FeatureCount { get { return featureEndIndex - featureStartIndex + 1; } }

            public int IdIndex { get { return idIndex; } }

            public int LabelIndex { get { return labelIndex; } }
        }
    }
}
