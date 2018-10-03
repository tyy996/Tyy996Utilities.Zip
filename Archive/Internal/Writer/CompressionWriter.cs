using System;
using System.Linq;
using System.IO;
using Unity.IO.Compression;

namespace Tyy996Utilities.Zip
{
    public partial class MyZipArchive
    {
        public sealed class CompressionWriter : Writer
        {
            public CompressionWriter(MyZipArchive archive, bool overwrite) : base(archive, overwrite)
            {
            }

            protected override byte[] makeHeader(MyArchiveFileHeader rawHeader, out byte basicType)
            {
                basicType = 1;
                var headerLength = rawHeader.RawHeader.Length;

                byte[] data = rawHeader.RawHeader.ToArray();

                translate(ref data);

                return data;
            }

            protected override void translate(ref byte[] data)
            {
                using (var compressStream = new MemoryStream())
                {
                    compressStream.Write(BitConverter.GetBytes(data.Length), 0, sizeof(int));
                    using (var compressor = new DeflateStream(compressStream, CompressionMode.Compress))
                    {
                        compressor.Write(data, 0, data.Length);
                    }

                    data = compressStream.ToArray();
                }
            }

            protected override Reader getReader()
            {
                return new CompressionReader(archive);
            }
        }
    }
}
