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
        public sealed class EncryptionWriter : Writer
        {
            private string keptKey;
            private ICryptoTransform crypto;

            public EncryptionWriter(MyZipArchive archive, bool overwrite, string key) : base(archive, overwrite)
            {
                keptKey = key;
                var rawKey = UTF8Encoding.UTF8.GetBytes(key);
                var key2 = new Rfc2898DeriveBytes(key, rawKey, 20);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = key2.GetBytes(tdes.KeySize / 8);
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                crypto = tdes.CreateEncryptor();
            }

            protected override byte[] makeHeader(MyArchiveFileHeader rawHeader, out byte basicType)
            {
                basicType = 2;
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
                        using (var cryptoStream = new CryptoStream(compressor, crypto, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(data, 0, data.Length);
                            cryptoStream.FlushFinalBlock();
                        }
                    }

                    data = compressStream.ToArray();
                }
            }

            protected override Reader getReader()
            {
                return new EncryptionReader(archive, keptKey);
            }
        }
    }
}
