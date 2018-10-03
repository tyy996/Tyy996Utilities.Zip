using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tyy996Utilities.Zip
{
    public struct HeaderInfo
    {
        public List<string> HeaderContents { get; private set; }
        public ArchiveType TypeOfArchive { get; private set; }

        public HeaderInfo(MyZipArchive.MyArchiveFileHeader value)
        {
            HeaderContents = value.HeaderOrder.Values.ToList();
            TypeOfArchive = value.TypeOfArchive;
        }
    }
}
