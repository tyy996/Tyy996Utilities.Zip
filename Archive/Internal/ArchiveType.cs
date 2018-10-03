

namespace Tyy996Utilities.Zip
{
    public class ArchiveType
    {
        public int DataType { get; private set; }
        public int BasicType { get { return DataType % 10; } }

        public ArchiveType(int type)
        {
            DataType = type;
        }
    }
}
