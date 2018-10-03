using System;
using System.Collections.Generic;

namespace Tyy996Utilities.Zip
{
    public partial class MyZipArchive
    {
        internal class DataBlockCollection
        {
            private LinkedList<DataBlock> blocks;
            //private int position;
            //private HashSet<string> nameSet;
            private Dictionary<string, DataBlock> blockLookup;

            public int BlockCount { get { return blocks.Count; } }
            public int DataLength { get { return blocks.Last.Value.EndPosition; } }

            public DataBlockCollection()
            {
                blocks = new LinkedList<DataBlock>();
                blockLookup = new Dictionary<string, DataBlock>();
            }

            public void Append(string internalFileName, byte[] data)
            {
                if (!blockLookup.TryAdd(internalFileName, new DataBlock(blocks, internalFileName, data)))
                {
                    blocks.RemoveLast();
                    throw new ArgumentException("File name already exists.");
                }
            }

            public bool Remove(string internalFileName)
            {
                DataBlock block;
                if (blockLookup.TryGetValue(internalFileName, out block))
                {
                    blockLookup.Remove(internalFileName);
                    blocks.Remove(block);
                    return true;
                }

                return false;
            }

            public void Edit(string internalFileName, byte[] data)
            {
                DataBlock block;
                if (blockLookup.TryGetValue(internalFileName, out block))
                    block.Data = data;
                else
                    throw new ArgumentException("File name doesn't exists.");
            }

            public void Assemble(out byte[] data, out MyArchiveFileHeader header)
            {
                data = new byte[DataLength];
                header = new MyArchiveFileHeader();

                int position;
                int length;
                foreach (DataBlock block in blocks)
                {
                    position = block.Position;
                    length = block.Length;

                    header.Add(block.FileName, position, length);
                    Array.Copy(block.Data, 0, data, position, length);
                }
            }

            public void Clear()
            {
                blocks.Clear();
                blockLookup.Clear();
            }
        }
    }
}
