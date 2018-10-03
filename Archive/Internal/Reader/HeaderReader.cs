using System;
using System.Text;
using System.IO;

namespace Tyy996Utilities.Zip
{
    public partial class MyZipArchive
    {
        public class HeaderReader : IDisposable
        {
            private bool isDisposed;

            protected MyZipArchive archive;
            protected MyArchiveFileHeader header;
            protected bool headerLoaded;
            protected int rawHeaderLength;
            protected int headerOffset;

            //public MyArchiveFileHeader Header { get { if (!headerLoaded) loadHeader(); return header; ; } }
            public bool IsDisposed { get { return isDisposed; } }

            public HeaderReader(MyZipArchive archive)
            {
                if (archive == null)
                    throw new ArgumentNullException();

                this.archive = archive;
            }

            ~HeaderReader()
            {
                Dispose();
            }

            public void Dispose()
            {
                if (isDisposed)
                    return;

                header.Dispose();

                isDisposed = true;
            }

            public ArchiveType GetArchiveType()
            {
                if (isDisposed)
                    throw new InvalidOperationException();

                if (!File.Exists(archive.FilePath))
                    throw new FileNotFoundException();

                if (headerLoaded)
                    return header.TypeOfArchive;

                ArchiveType type;
                using (FileStream fileStream = File.Open(archive.FilePath, FileMode.Open))
                {
                    using (BinaryReader reader = new BinaryReader(fileStream))
                    {
                        type = new ArchiveType(reader.ReadInt32());
                    }
                }

                return type;
            }

            public bool GetHeader(out HeaderInfo header)
            {
                if (headerLoaded)
                {
                    header = new HeaderInfo(this.header);
                    return true;
                }
                else
                {
                    var result = loadHeader();
                    header = new HeaderInfo(this.header);
                    return result;
                }
            }

            #region Header

            protected bool loadHeader()
            {
                if (isDisposed)
                    throw new InvalidOperationException();

                if (headerLoaded || !File.Exists(archive.FilePath))
                    return false;

                try
                {
                    header = new MyArchiveFileHeader();
                    readHeader();
                    headerLoaded = true;
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            private void readHeader()
            {
                byte[] rawHeader;
                int headerLength;

                using (FileStream fileStream = File.Open(archive.FilePath, FileMode.Open))
                {
                    using (BinaryReader reader = new BinaryReader(fileStream))
                    {
                        header.TypeOfArchive = new ArchiveType(reader.ReadInt32());
                        rawHeaderLength = reader.ReadInt32();
                        rawHeader = reader.ReadBytes(rawHeaderLength);
                    }
                }
                headerOffset = sizeof(int) * 2; //header length and archive type (both ints)

                rawHeader = translateHeader(rawHeader, out headerLength);
                StringBuilder stringBuilder = new StringBuilder();

                using (MemoryStream stream = new MemoryStream(rawHeader))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        for (int headerCount = 0; headerCount < headerLength;)
                        {
                            stringBuilder.Length = 0;
                            int nameLength = reader.ReadInt32();
                            headerCount += sizeof(int);

                            stringBuilder.Append(reader.ReadChars(nameLength));
                            headerCount += sizeof(char) * nameLength;

                            header.Add(stringBuilder.ToString(), reader.ReadInt32(), reader.ReadInt32());
                            headerCount += sizeof(int) * 2;
                        }
                    }
                }
            }

            protected virtual byte[] translateHeader(byte[] raw, out int length)
            {
                length = raw.Length;
                return raw;
            }
            #endregion
        }
    }
}
