using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Tyy996Utilities.Zip.Utility
{
    public static class MyByteConverter
    {
        public static byte[] ToByteArray(object source)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, source);
                return stream.ToArray();
            }
        }

        public static T ToObject<T>(byte[] rawData)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream(rawData))
            {
                return (T)formatter.Deserialize(stream);
            }
        }

        public static object ToObject(byte[] rawData)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream(rawData))
            {
                return formatter.Deserialize(stream);
            }
        }
    }
}
