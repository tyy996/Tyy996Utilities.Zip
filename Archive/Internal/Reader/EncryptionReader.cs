using System;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace Tyy996Utilities.Zip
{
    public partial class MyZipArchive
    {
        public sealed class EncryptionReader : Reader
        {
            private ICryptoTransform crypto;

            public EncryptionReader(MyZipArchive archive, string key) : base(archive)
            {
                var rawKey = UTF8Encoding.UTF8.GetBytes(key);
                var key2 = new Rfc2898DeriveBytes(key, rawKey, 20);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = key2.GetBytes(tdes.KeySize / 8);
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                crypto = tdes.CreateDecryptor();
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
                        using (var cryptoStream = new CryptoStream(zipStream, crypto, CryptoStreamMode.Read))
                        {
                            //outBuffer = new byte[2048];
                            if (!reinsertBlockLength)
                                cryptoStream.Read(outBuffer, 0, blockLength);
                            else
                                cryptoStream.Read(outBuffer, sizeof(int), blockLength);
                        }
                    }

                    data = outBuffer;
                }
            }
        }
    }
}
