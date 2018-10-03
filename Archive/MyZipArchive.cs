using System.IO;

namespace Tyy996Utilities.Zip
{
    public partial class MyZipArchive
    {
        public string FileName { get; private set; }
        public string FolderDirectory { get; private set; }
        public string FilePath { get { return Path.Combine(FolderDirectory, FileName); } }
        public virtual int ArchiveType { get { return 0; } }

        public MyZipArchive(string folderDirectory, string fileName)
        {
            FolderDirectory = folderDirectory;
            FileName = fileName;
        }

        public static Reader GetReader(MyZipArchive archive, string encryptionKey = "")
        {
            int basicType;

            using (var reader = new HeaderReader(archive))
            {
                basicType = reader.GetArchiveType().BasicType;
            }

            switch (basicType)
            {
                case 0:
                    return new Reader(archive);

                case 1:
                    return new CompressionReader(archive);

                case 2:
                    return new EncryptionReader(archive, encryptionKey);

                default:
                    return null;
            }
        }

        //public static bool TryGetReader(MyZipArchive archive, out Reader reader)
        //{

        //}

        public static Writer GetWriter(MyZipArchive archive, bool overwrite, string encryptionKey = "")
        {
            int basicType;

            using (var reader = new HeaderReader(archive))
            {
                basicType = reader.GetArchiveType().BasicType;
            }

            switch (basicType)
            {
                case 0:
                    return new Writer(archive, overwrite);

                case 1:
                    return new CompressionWriter(archive, overwrite);

                case 2:
                    return new EncryptionWriter(archive, overwrite, encryptionKey);

                default:
                    return null;
            }
        }
    }
}
