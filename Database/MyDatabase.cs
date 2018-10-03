using System.IO;

namespace Tyy996Utilities.Zip.Database
{
    public abstract class MyDatabase : MyZipArchive
    {
        private string versionNumber;

        public bool VersionLoaded { get; protected set; }
        public string VersionNumber { get { return versionNumber; } protected set { versionNumber = value; } }

        public MyDatabase(string folderDirectory, string fileName, string encryptionKey) :
            base(folderDirectory, fileName)
        {
            VersionLoaded = loadVersion(folderDirectory, fileName, defaultLoadVersion, encryptionKey);
        }

        public MyDatabase(string folderDirectory, string fileName, LoadVersionProcess process, string encryptionKey) :
            base(folderDirectory, fileName)
        {
            VersionLoaded = loadVersion(folderDirectory, fileName, process, encryptionKey);
        }

        private bool loadVersion(string folderDirectory, string fileName, LoadVersionProcess process, string encryptionKey)
        {
            if (!File.Exists(Path.Combine(folderDirectory, fileName)))
                return false;

            return process(GetReader(this, encryptionKey), out versionNumber);
        }

        public void CreateDatabase(string version)
        {
            CreateDatabase(version, defaultCreateDatabase);
        }

        public void CreateDatabase(string version, CreateDatabaseProcess process)
        {
            process(version);
            versionNumber = version;
            VersionLoaded = true;
        }

        public object LoadData(LoadDataProcess process, string encryptionKey = "")
        {
            return process(GetReader(this, encryptionKey));
        }

        public void Upgrade(UpgradeDatabaseProcess process)
        {
            process(this);
        }

        protected abstract bool defaultLoadVersion(Reader reader, out string version);
        protected abstract void defaultCreateDatabase(string version);
    }
}
