using System;
using System.IO;
using Unity.IO.Compression;

namespace Tyy996Utilities.Zip
{
    public partial class MyZipArchive
    {
        public sealed class CompressionReader : Reader
        {
            public CompressionReader(MyZipArchive archive) : base(archive)
            {
            }

            protected override byte[] translateHeader(byte[] raw, out int length)
            {
                translateBlock(ref raw, true);

                using (MemoryStream memory = new MemoryStream(raw))
                {
                    using (BinaryReader reader = new BinaryReader(memory))
                    {
                        length = reader.ReadInt32();
                        //headerOffset += sizeof(int);

                        raw = reader.ReadBytes(raw.Length - sizeof(int));
                    }
                }

                return raw;
            }

            protected override void translateBlock(ref byte[] data, bool reinsertBlockLength)
            {
                int blockLength = BitConverter.ToInt32(data, 0);
                byte[] outBuffer;

                if (reinsertBlockLength)
                    outBuffer = new byte[blockLength + sizeof(int)];
                else
                    outBuffer = new byte[blockLength];

                using (MemoryStream memory = new MemoryStream(data))
                {
                    if (reinsertBlockLength)
                        memory.Read(outBuffer, 0, sizeof(int));
                    else
                        memory.Position = sizeof(int);

                    using (var zipStream = new DeflateStream(memory, CompressionMode.Decompress))
                    {
                        if (!reinsertBlockLength)
                            zipStream.Read(outBuffer, 0, blockLength);
                        else
                            zipStream.Read(outBuffer, sizeof(int), blockLength);
                    }

                    data = outBuffer;
                }
            }
        }
    }
}
