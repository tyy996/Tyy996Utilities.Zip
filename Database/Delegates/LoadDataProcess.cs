
namespace Tyy996Utilities.Zip.Database
{
    public delegate object LoadDataProcess(MyZipArchive.Reader reader);
    public delegate T LoadDataProcess<T>(MyZipArchive.Reader reader);
    public delegate void LoadDataIntoProcess<T>(MyZipArchive.Reader reader, ref T value);
}
