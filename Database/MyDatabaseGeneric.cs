
namespace Tyy996Utilities.Zip.Database
{
    public abstract class MyDatabase<T> : MyDatabase
    {
        public MyDatabase(string folderDirectory, string fileName, string encryptionKey) :
            base(folderDirectory, fileName, encryptionKey)
        {
        }

        public MyDatabase(string folderDirectory, string fileName, LoadVersionProcess process, string encryptionKey) :
            base(folderDirectory, fileName, process, encryptionKey)
        {
        }

        public T LoadData(LoadDataProcess<T> process, string encryptionKey = "")
        {
            return process(GetReader(this, encryptionKey));
        }

        public void LoadDataInto(LoadDataIntoProcess<T> process, ref T value, string encryptionKey = "")
        {
            process(GetReader(this, encryptionKey), ref value);
        }
    }
}
