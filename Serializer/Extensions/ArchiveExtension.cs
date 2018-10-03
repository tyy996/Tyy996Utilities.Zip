using Tyy996Utilities.Zip.Utility;

namespace Tyy996Utilities.Zip.Serializer
{
    public static class ArchiveExtension
    {
        public static void WriteFileToArchive(this MyZipArchive.Writer writer, string fileName, PrimitiveData data)
        {
            writer.WriteFileToArchive(fileName, MyByteConverter.ToByteArray(data));
        }

        public static bool ReadInternalFile(this MyZipArchive.Reader reader, string internalFileName, out PrimitiveData data)
        {
            byte[] raw;
            if (reader.ReadInternalFile(internalFileName, out raw))
            {
                data = MyByteConverter.ToObject<PrimitiveData>(raw);
                return true;
            }

            data = new PrimitiveData();
            return false;
        }
    }
}
