using System;
using System.Collections.Generic;
using System.IO;

namespace Tyy996Utilities.Zip
{
    public partial class MyZipArchive
    {
        public class Writer : IDisposable
        {
            private DataBlockCollection _blocks;

            protected MyZipArchive archive;
            private bool hasTriedToLoad;
            private writerState state; //overkill, but I was bored writhing the rest of the class
            private bool isDisposed { get { return state == writerState.Disposed; } set { state = writerState.Disposed; } }
            private bool overwrite { get { return (state & writerState.Overwrite) == writerState.Overwrite; } set { if (value) state |= (state == 0 ? 0 : writerState.Overwrite); } }
            private bool hasLoaded { get { return (state & writerState.Opended) == writerState.Opended; } set { if (value) state |= (state <= writerState.Overwrite ? state : writerState.Opended); } }
            private bool isUpdating { get { return (state & writerState.Updating) == writerState.Updating; } set { if (value) state |= (state <= writerState.Overwrite ? state : writerState.Updating); } }
            private bool isAppending { get { return (state & writerState.Appending) == writerState.Appending; } set { if (value) state |= (state <= writerState.Overwrite ? state : writerState.Appending); } }
            private DataBlockCollection blocks { get { return _blocks ?? (_blocks = overwrite ? _blocks = new DataBlockCollection() : tryLoadFile()); } }

            public Writer(MyZipArchive archive, bool overwrite)
            {
                if (archive == null)
                    throw new ArgumentNullException();

                this.archive = archive;
                state = writerState.Waiting;
                this.overwrite = overwrite;

                //if (!overwrite)
                //    tryLoadFile();
                //else
                //    blocks = new DataBlockCollection();
            }

            ~Writer()
            {
                Dispose();
            }

            public void Dispose()
            {
                if (isDisposed)
                    return;

                blocks.Clear();
                isDisposed = true;
            }

            #region Write
            public void WriteFileToArchive(string fileName, byte[] data)
            {
                writeBlock(fileName, data);
            }

            public bool TryWriteFileToArchive(string fileName, byte[] data)
            {
                try
                {
                    writeBlock(fileName, data);
                    return true;
                }
                catch (ArgumentException)
                {
                    return false;
                }
            }

            private void writeBlock(string fileName, byte[] data)
            {
                translate(ref data);
                blocks.Append(fileName, data);
            }

            protected virtual void translate(ref byte[] data)
            {

            }
            #endregion

            #region Edit
            public void Edit(string internalFileName, byte[] data)
            {
                edit(internalFileName, data);
            }

            public bool TryEdit(string internalFileName, byte[] data)
            {
                try
                {
                    edit(internalFileName, data);
                    return true;
                }
                catch (ArgumentException)
                {
                    return false;
                }
            }
                

            private void edit(string internalFileName, byte[] data)
            {
                translate(ref data);
                blocks.Edit(internalFileName, data);
            }
            #endregion

            #region Remove
            public bool Remove(string internalFileName)
            {
                return blocks.Remove(internalFileName);
            }
            #endregion

            #region Compress

            public void Compress()
            {
                compressTempFile();
            }

            protected virtual byte[] makeHeader(MyArchiveFileHeader rawHeader, out byte basicType)
            {
                basicType = 0;
                return rawHeader.RawHeader.ToArray();
            }

            private void compressTempFile()
            {
                var tempFileName = Guid.NewGuid().ToString() + ".tmp";
                var tempFilePath = Path.Combine(archive.FolderDirectory, tempFileName);

                MyArchiveFileHeader rawHeader;
                byte[] assembledData;

                if (blocks.BlockCount == 0)
                    return;

                blocks.Assemble(out assembledData, out rawHeader);

                byte basicType;
                var header = makeHeader(rawHeader, out basicType);

                using (FileStream fileStream = File.Open(tempFilePath, FileMode.Create))
                {
                    using (BinaryWriter writer = new BinaryWriter(fileStream))
                    {
                        //type writes here only
                        writer.Write(archive.ArchiveType + basicType);
                        writer.Write((int)header.Length);
                        writer.Write(header);
                        writer.Write(assembledData);
                    }
                }

                switchOutTemp(tempFilePath);
            }

            private void switchOutTemp(string tempFilePath)
            {
                File.Delete(archive.FilePath);
                File.Move(tempFilePath, archive.FilePath);
            }
            #endregion

            private DataBlockCollection tryLoadFile()
            {
                bool result;
                DataBlockCollection blockResult;
                using (var reader = getReader())
                {
                    result = reader.ReadFullFile(out blockResult);
                }

                if (result)
                {
                    hasLoaded = true;
                }
                else
                {
                    overwrite = true;
                }

                return blockResult;
            }

            protected virtual Reader getReader()
            {
                return new Reader(archive);
            }

            [Flags]
            private enum writerState
            {
                Disposed = 0,
                Overwrite = 1,
                Opended = 1 << 2,
                Appending = 1 << 3,
                Updating = 1 << 4,
                Waiting = 1 << 5
            }
        }
    }
}
