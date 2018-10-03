using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Tyy996Utilities.Zip
{
    public partial class MyZipArchive
    {
        public class Reader : HeaderReader //: IDisposable
        {
            //private bool isDisposed;
            //protected MyZipArchive archive;
            //private MyArchiveFileHeader header;
            //private bool headerLoaded;
            //private int rawHeaderLength;

            //protected int headerOffset;

            //public bool CanRead { get; private set; }

            public Reader(MyZipArchive archive) : base(archive)
            {
                //if (archive == null)
                //    throw new ArgumentNullException();

                //this.archive = archive;
            }

            //~Reader()
            //{
            //    Dispose();
            //}

            //public void Dispose()
            //{
            //    if (isDisposed)
            //        return;

            //    header.Dispose();

            //    isDisposed = true;
            //}

            //#region Header

            //private bool loadHeader()
            //{
            //    if (isDisposed)
            //        throw new InvalidOperationException();

            //    if (headerLoaded && !File.Exists(archive.FilePath))
            //        return false;

            //    try
            //    {
            //        header = new MyArchiveFileHeader();
            //        readHeader();
            //        headerLoaded = true;
            //        return true;
            //    }
            //    catch(Exception)
            //    {
            //        return false;
            //    }
            //}

            //private void readHeader()
            //{
            //    byte[] rawHeader;
            //    int headerLength;

            //    using (FileStream fileStream = File.Open(archive.FilePath, FileMode.Open))
            //    {
            //        using (BinaryReader reader = new BinaryReader(fileStream))
            //        {
            //            rawHeaderLength = reader.ReadInt32();
            //            rawHeader = reader.ReadBytes(rawHeaderLength);
            //        }
            //    }
            //    headerOffset = sizeof(int);

            //    rawHeader = translateHeader(rawHeader, out headerLength);
            //    StringBuilder stringBuilder = new StringBuilder();

            //    using (MemoryStream stream = new MemoryStream(rawHeader))
            //    {
            //        using (BinaryReader reader = new BinaryReader(stream))
            //        {
            //            for (int headerCount = 0; headerCount < headerLength;)
            //            {
            //                stringBuilder.Length = 0;
            //                int nameLength = reader.ReadInt32();
            //                headerCount += sizeof(int);

            //                stringBuilder.Append(reader.ReadChars(nameLength));
            //                headerCount += sizeof(char) * nameLength;

            //                header.Add(stringBuilder.ToString(), reader.ReadInt32(), reader.ReadInt32());
            //                headerCount += sizeof(int) * 2;
            //            }
            //        }
            //    }
            //}

            //protected virtual byte[] translateHeader(byte[] raw, out int length)
            //{
            //    length = raw.Length;
            //    return raw;
            //}
            //#endregion

            #region Read Block
            public bool ReadInternalFile(string internalFileName, out byte[] data)
            {
                if (IsDisposed)
                    throw new InvalidOperationException();

                data = null;

                if (!File.Exists(archive.FilePath))
                    return false;

                if (!headerLoaded)
                    if (!loadHeader())
                        throw new InvalidDataException("Header is missing from file.");

                try
                {
                    int position;
                    int length;
                    if (header.TryGet(internalFileName, out position, out length))
                    {
                        readBlock(position, length, out data);
                        return true;
                    }
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            private void readBlock(int position, int length, out byte[] data)
            {
                readRawBlock(position, length, out data);
                translateBlock(ref data, false);
            }

            private void readRawBlock(int position, int length, out byte[] data)
            {
                using (FileStream fileStream = File.Open(archive.FilePath, FileMode.Open))
                {
                    fileStream.Position = rawHeaderLength + position + headerOffset;
                    using (BinaryReader reader = new BinaryReader(fileStream))
                    {
                        data = reader.ReadBytes(length);
                    }
                }
            }

            protected virtual void translateBlock(ref byte[] data, bool reinsertBlockLength)
            {
            }
            #endregion

            #region Read File

            internal bool ReadFullFile(out DataBlockCollection blocks)
            {
                if (IsDisposed)
                    throw new InvalidOperationException();

                blocks = new DataBlockCollection();

                if (!File.Exists(archive.FilePath))
                    return false;

                if (!headerLoaded)
                    if (!loadHeader())
                        throw new InvalidDataException("Header is missing from file.");

                try
                {
                    //blocks = new DataBlockCollection();
                    readAllBlocks(ref blocks);
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            private void readAllBlocks(ref DataBlockCollection blocks)
            {
                int position;
                int length;
                byte[] data;

                foreach (string fileName in header.HeaderOrder.Values)
                {
                    header.TryGet(fileName, out position, out length);
                    //readBlock(position, length, out data);
                    readRawBlock(position, length, out data);
                    blocks.Append(fileName, data);
                }
            }
            #endregion
        }
    }
}
