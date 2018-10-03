using System;
using System.Collections.Generic;
using System.IO;

namespace Tyy996Utilities.Zip
{
    public partial class MyZipArchive
    {
        public struct MyArchiveFileHeader : IDisposable
        {
            private const int TEMP_STREAM_BUFFER = 512;

            private bool isDisposed;
            private Dictionary<string, MyArchiveFileHeaderElement> _headerLookup;
            private SortedList<int, string> headerOrder;
            private MemoryStream rawHeader;

            private Dictionary<string, MyArchiveFileHeaderElement> headerLookup { get { return _headerLookup ?? (_headerLookup = new Dictionary<string, MyArchiveFileHeaderElement>()); } }

            public SortedList<int, string> HeaderOrder { get { if (isDisposed) throw new InvalidOperationException(); return headerOrder ?? (headerOrder = new SortedList<int, string>()); } }
            //public Dictionary<string, MyArchiveFileHeaderElement> HeaderLookup { get { if (isDisposed) throw new InvalidOperationException(); return headerLookup ?? (headerLookup = new Dictionary<string, MyArchiveFileHeaderElement>()); } }
            public MemoryStream RawHeader { get { if (isDisposed) throw new InvalidOperationException(); return rawHeader ?? (rawHeader = new MemoryStream()); } }
            public ArchiveType TypeOfArchive { get; set; }
            //public int CompressedHeaderLength { get; set; }

            public void Dispose()
            {
                if (isDisposed)
                    return;

                headerLookup.Clear();
                HeaderOrder.Clear();
                RawHeader.Close();
                RawHeader.Dispose();

                isDisposed = true;
            }

            public void Add(string internalFileName, int position, int length)
            {
                if (isDisposed)
                    throw new InvalidOperationException();

                headerLookup.Add(internalFileName, new MyArchiveFileHeaderElement(position, length));
                HeaderOrder.Add(position, internalFileName);

                using (MemoryStream tempStream = new MemoryStream(TEMP_STREAM_BUFFER))
                {
                    using (BinaryWriter writer = new BinaryWriter(tempStream))
                    {
                        writer.Write(internalFileName.Length);

                        foreach (char c in internalFileName)
                            writer.Write(c);

                        writer.Write(position);
                        writer.Write(length);

                        writer.Flush();

                        var buffer = tempStream.ToArray();
                        RawHeader.Write(buffer, 0, buffer.Length);
                    }
                }
            }

            public bool TryGet(string internalFileName, out int position, out int length)
            {
                MyArchiveFileHeaderElement element;
                if (headerLookup.TryGetValue(internalFileName, out element))
                {
                    position = element.Position;
                    length = element.Length;
                    return true;
                }
                else
                {
                    position = 0;
                    length = 0;
                    return false;
                }
            }

            private struct MyArchiveFileHeaderElement
            {
                public int Position { get; set; }
                public int Length { get; set; }

                public MyArchiveFileHeaderElement(int position, int lenth)
                {
                    Position = position;
                    Length = lenth;
                }
            }
        }
    }
}
