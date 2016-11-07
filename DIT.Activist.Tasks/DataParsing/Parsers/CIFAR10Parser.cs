using DIT.Activist.Domain.Interfaces.Data;
using DIT.Activist.Tasks.DataParsing.Formats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Tasks.DataParsing.Parsers
{
    public class CIFAR10Parser : IDataParser
    {
        private const PixelFormat CIFAR_IMAGE_PIXEL_FORMAT = PixelFormat.Format32bppArgb;
        private static int imageCount = 1;
        private CIFAR10Format format = new CIFAR10Format();

        public CIFAR10Parser() { }

        public IDataFormat Format
        {
            get { return format; }
        }

        public IEnumerable<object[]> ExtractFeatureValues(Stream byteStream, int limit=-1)
        {
            //the first byte is the label, skip this
            const int offset = 0;
            int bytesRead = 0;
            int imageCount = 0;

            byte[] buffer = new byte[CIFAR10Format.CIFAR_IMAGE_NUM_BYTES];

            while ((bytesRead = byteStream.Read(buffer, offset, CIFAR10Format.CIFAR_IMAGE_NUM_BYTES)) != 0)
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

        public IEnumerable<KeyValuePair<object, object>> ExtractLabels(Stream byteStream)
        {
            const int offset = 0;
            int bytesRead = 0;
            int imageCount = 0;

            byte[] buffer = new byte[CIFAR10Format.CIFAR_IMAGE_NUM_BYTES];

            while ((bytesRead = byteStream.Read(buffer, offset, CIFAR10Format.CIFAR_IMAGE_NUM_BYTES)) != 0)
            {
                yield return new KeyValuePair<object, object>(++imageCount, buffer[0]);
            }
        }

        public IEnumerable<object[]> ExtractFeaturesAndLabels(Stream byteStream, int limit=-1)
        {
            const int offset = 0;
            int bytesRead;
            int imageCount = 0;

            byte[] buffer = new byte[CIFAR10Format.CIFAR_IMAGE_NUM_BYTES];

            while ((bytesRead = byteStream.Read(buffer, offset, CIFAR10Format.CIFAR_IMAGE_NUM_BYTES)) != 0)
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

        private string BytesToBase64Png(byte[] buffer)
        {
            IList<byte> rChannel = new ArraySegment<byte>(buffer, 1, CIFAR10Format.CIFAR_IMAGE_CHANNEL_SIZE);
            IList<byte> gChannel = new ArraySegment<byte>(buffer, 1 + (CIFAR10Format.CIFAR_IMAGE_CHANNEL_SIZE), CIFAR10Format.CIFAR_IMAGE_CHANNEL_SIZE);
            IList<byte> bChannel = new ArraySegment<byte>(buffer, 1 + (CIFAR10Format.CIFAR_IMAGE_CHANNEL_SIZE * 2), CIFAR10Format.CIFAR_IMAGE_CHANNEL_SIZE);
            int label = buffer[0];

            using (Bitmap b = new Bitmap(CIFAR10Format.CIFAR_IMAGE_WIDTH, CIFAR10Format.CIFAR_IMAGE_HEIGHT, CIFAR_IMAGE_PIXEL_FORMAT))
            {
                int ii, jj;
                int sourceOffset = 0;
                for (ii = 0; ii < CIFAR10Format.CIFAR_IMAGE_WIDTH; ii++)
                {
                    for (jj = 0; jj < CIFAR10Format.CIFAR_IMAGE_WIDTH; jj++)
                    {
                        b.SetPixel(ii, jj, Color.FromArgb(255, rChannel[sourceOffset], gChannel[sourceOffset], bChannel[sourceOffset]));
                        sourceOffset++;
                    }
                }

                //b.Save(@"C:\Users\Jack\Desktop\cifar\test_" + imageCount++ + "_" + label + ".png", System.Drawing.Imaging.ImageFormat.Png);

                using (MemoryStream pngBuffer = new MemoryStream())
                {
                    b.Save(pngBuffer, System.Drawing.Imaging.ImageFormat.Png);

                    return System.Convert.ToBase64String(pngBuffer.ToArray());
                }
            }
        }
    }
}
