using System;
using System.IO;

namespace Tyy996Utilities.Zip.Utility
{
    public class NonClosingStreamDecorator : Stream
    {
        private readonly Stream innerStream;

        public NonClosingStreamDecorator(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            innerStream = stream;
        }

        public override bool CanRead { get { return innerStream.CanRead; } }

        public override bool CanSeek { get { return innerStream.CanSeek; } }

        public override bool CanWrite { get { return innerStream.CanWrite; } }

        public override long Length { get { return innerStream.Length; } }

        public override long Position { get { return innerStream.Position; } set { innerStream.Position = value; } }

        public override void Close()
        {
            // do not delegate !!
            //_innerStream.Close();
        }

        public override void Flush()
        {
            innerStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return innerStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return innerStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            innerStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            innerStream.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            // do not delegate !!
            //_innerStream.Dispose();
        }
    }
}
