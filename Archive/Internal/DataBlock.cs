using System;
using System.Collections.Generic;

namespace Tyy996Utilities.Zip
{
    public partial class MyZipArchive
    {
        private sealed class DataBlock
        {
            private LinkedListNode<DataBlock> node;

            public byte[] Data { get; set; }
            public int Position { get { return node.Previous == null ? 0 : node.Previous.Value.EndPosition; } }
            public int EndPosition { get { return Position + Length; } }
            public int Length { get { return Data.Length; } }
            public string FileName { get; private set; }

            public DataBlock(LinkedList<DataBlock> list, string fileName, byte[] data)
            {
                node = list.AddLast(this);
                Data = data;
                FileName = fileName;
            }
        }
    }
}
