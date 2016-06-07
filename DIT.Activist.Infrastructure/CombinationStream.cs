using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Infrastructure
{
    public class CombinationStream : Stream, IDisposable
    {
        private Stream[] source;
        private long[] streamLengths;
        private long position = 0;
        private long length;

        public CombinationStream(IEnumerable<Stream> source)
        {
            this.source = source.ToArray();
            streamLengths = source.Select(s => s.Length).ToArray();
            length = streamLengths.Sum();
        }

        private Tuple<int, long> GetStreamIndexAndPosition(long position)
        {
            int currentIndex = 0;
            long totalLength = streamLengths[currentIndex];
            while (position > totalLength)
            {
                currentIndex++;
                if (currentIndex == streamLengths.Length)
                {
                    return new Tuple<int, long>(currentIndex, 0);
                }
                totalLength += streamLengths[currentIndex];
            }

            long totalOffset = position - (totalLength - streamLengths[currentIndex]);

            return new Tuple<int, long>(currentIndex, totalOffset);
        }

        public override bool CanRead { get { return true; } }

        public override bool CanSeek { get { return true; } }

        public override bool CanWrite { get { return false; } }

        public override long Length
        {
            get
            {
                return length;
            }
        }

        public override long Position
        {
            get
            {
                return position;
            }

            set
            {
                position = value;
            }
        }

        public override void Flush()
        {
            foreach (Stream stream in source)
            {
                stream.Flush();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;
            int currentOffset = offset;
            int bytesToRead = count;

            while (totalBytesRead < bytesToRead)
            {
                
                //locate the correct stream and offset to begin reading
                var streamIndexAndPosition = GetStreamIndexAndPosition(position);
                int currentStreamIndex = streamIndexAndPosition.Item1;
                long positionInCurrentStream = streamIndexAndPosition.Item2;

                if (currentStreamIndex == source.Length)
                {
                    break;
                }
                //we need to seek the current stream to its offset
                Stream currentStream = source[currentStreamIndex];
                currentStream.Seek(positionInCurrentStream, SeekOrigin.Begin);

                //read from the current stream
                int bytesRead = currentStream.Read(buffer, currentOffset, bytesToRead);

                //we need this to know when we're done
                totalBytesRead += bytesRead;
                //we need to increment the offset so we begin writing to the buffer in the correct
                //place on the next iteration
                currentOffset += bytesRead;
                //update our internal position counter
                position += bytesRead;
                //update the number of bytes we still need to read
                bytesToRead -= bytesRead;
                if (bytesRead == 0)
                {
                    position += 1;
                }
            }

            return totalBytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (origin == SeekOrigin.Begin)
            {
                Position = offset;
            }
            else if (origin == SeekOrigin.Current)
            {
                Position += offset;
            }
            else if (origin == SeekOrigin.End)
            {
                Position = Length - offset;
            }

            return Position;
        }

        public override void SetLength(long value)
        {
            length = value;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new System.InvalidOperationException();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach(Stream stream in source)
                    {
                        stream.Dispose();
                    }
                }

                disposedValue = true;
            }
        }

        #endregion
    }
}
