using System;
using System.IO;
using System.IO.Compression;

namespace Kemorave.IO.Zip
{
    public class Zipper : Progress<int>
    {
        private bool _isCancelled;
        public Zipper() { }
        public Zipper(Action<int> progressHandler) : base(progressHandler) { }

        public Zipper(string destinationFilePath)
        {
            DestinationFilePath = destinationFilePath;
        }

        public void CompressFile(string path)
        {
            using (FileStream dfs = new FileStream(DestinationFilePath, OverrideExistingFiles ? FileMode.Create : FileMode.CreateNew))
            {
                using (ZipArchive zipArchive = new ZipArchive(dfs, ZipArchiveMode.Create))
                {

                    DoCreateEntryFromFile(zipArchive, path, Path.GetFileName(path), this.CompressionLevel);

                }
            }
        }

        public void DecompressFile(string path)
        {
            using (FileStream dfs = new FileStream(path, FileMode.Open))
            {
                DecompressFile(dfs);
            }
        }
        public void DecompressFile(Stream fileStream)
        {
            Decompress(fileStream);
        }

        public void ExtractToDirectory(ZipArchive source)
        {
            _isCancelled = false;
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (DestinationFilePath == null)
            {
                throw new ArgumentNullException("destinationDirectoryName");
            }
            DirectoryInfo directoryInfo = Directory.CreateDirectory(DestinationFilePath.Substring(0, DestinationFilePath.Length - DefaultExtintion.Length));
            string text = directoryInfo.FullName;
            foreach (ZipArchiveEntry entry in source.Entries)
            {
                if (_isCancelled)
                {
                    return;
                }
                string fullPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(text, entry.FullName));
                if (!fullPath.StartsWith(text, StringComparison.OrdinalIgnoreCase))
                {
                    throw new IOException(("IO_ExtractingResultsInOutside"));
                }
                if (Path.GetFileName(fullPath).Length == 0)
                {
                    if (entry.Length != 0L)
                    {
                        throw new IOException(("IO_DirectoryNameWithData"));
                    }
                    Directory.CreateDirectory(fullPath);
                }
                else
                {
                    Directory.CreateDirectory(Path.GetParentPath(fullPath));
                    ExtractToFile(entry, fullPath, OverrideExistingFiles);
                }
            }
        }

        public void CompressDirectory(string dir)
        {
            using (FileStream dfs = new FileStream(DestinationFilePath, OverrideExistingFiles ? FileMode.Create : FileMode.CreateNew))
            {
                using (ZipArchive zipArchive = new ZipArchive(dfs, ZipArchiveMode.Create))
                {
                    ConstructArchive(dir, dir, zipArchive);
                }

            }
        }

        public void CancelPendingWork()
        {
            _isCancelled = true;
        }

        private void Decompress(Stream fileStream)
        {
            using (ZipArchive com = new ZipArchive(fileStream, ZipArchiveMode.Read))
            {
                ExtractToDirectory(com);
            }
        }

        private void DoStreamCopy(Stream from, Stream to, long max)
        {
            long writtenbytes = 0;
            byte[] buffer = new byte[BufferSize];
            while (from.Read(buffer, 0, BufferSize) > 0 && !_isCancelled)
            {
                to.Write(buffer, 0, BufferSize);
                writtenbytes += buffer.LongLength;
                OnReport((int)Math.Round((double)(writtenbytes * 100 / max), 2));
            }
        }
        private void ConstructArchive(string root, string dir, ZipArchive zipArchive)
        {
            _isCancelled = false;
            string dirName = Path.GetFileName(dir), rootName = Path.GetFileName(root);
            if (dirName != rootName)
            {
                dirName = "";
                foreach (string item in dir.Substring((root.Length - rootName.Length), (dir.Length - root.Length + rootName.Length)).Split('\\'))
                {
                    if (_isCancelled)
                    {
                        return;
                    }
                    dirName += "/" + item;
                }
                dirName = dirName.Substring(1, dirName.Length - 1);
            }
            foreach (string file in Directory.EnumerateFiles(dir))
            {
                if (_isCancelled)
                {
                    return;
                }
                string fileName = Path.GetFileName(file);
                DoCreateEntryFromFile(zipArchive, file, dirName + "/" + fileName, this.CompressionLevel);
            }
            foreach (string supDir in Directory.EnumerateDirectories(dir))
            {
                if (_isCancelled)
                {
                    return;
                }
                ConstructArchive(root, supDir, zipArchive);
            }
        }
        private void ExtractToFile(ZipArchiveEntry source, string destinationFileName, bool overwrite)
        {
            _isCancelled = false;
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (destinationFileName == null)
            {
                throw new ArgumentNullException("destinationFileName");
            }
            FileMode mode = (!overwrite) ? FileMode.CreateNew : FileMode.Create;
            using (Stream destination = System.IO.File.Open(destinationFileName, mode, FileAccess.Write, FileShare.None))
            {
                using (Stream stream = source.Open())
                {
                    CurrentFile = destinationFileName;
                    DoStreamCopy(stream, destination, source.Length);
                }
            }
            System.IO.File.SetLastWriteTime(destinationFileName, source.LastWriteTime.DateTime);
        }
        private ZipArchiveEntry DoCreateEntryFromFile(ZipArchive destination, string sourceFileName, string entryName, CompressionLevel? compressionLevel)
        {
            _isCancelled = false;
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }
            if (sourceFileName == null)
            {
                throw new ArgumentNullException("sourceFileName");
            }
            if (entryName == null)
            {
                throw new ArgumentNullException("entryName");
            }
            using (Stream stream = System.IO.File.Open(sourceFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ZipArchiveEntry zipArchiveEntry = compressionLevel.HasValue ? destination.CreateEntry(entryName, compressionLevel.Value) : destination.CreateEntry(entryName);
                DateTime dateTime = System.IO.File.GetLastWriteTime(sourceFileName);
                if (dateTime.Year < 1980 || dateTime.Year > 2107)
                {
                    dateTime = new DateTime(1980, 1, 1, 0, 0, 0);
                }
                zipArchiveEntry.LastWriteTime = dateTime;

                using (Stream destination2 = zipArchiveEntry.Open())
                {
                    CurrentFile = sourceFileName;
                    DoStreamCopy(stream, destination2, stream.Length);
                }
                return zipArchiveEntry;
            }
        }

        public int BufferSize { get; set; } = 1024;
        public bool OverrideExistingFiles { get; set; } = true;

        public string DestinationFilePath
        {
            get => _DestinationFilePath;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("Destination File Path");
                }
                _DestinationFilePath = value;
                if (!_DestinationFilePath.EndsWith(DefaultExtintion))
                {
                    _DestinationFilePath += DefaultExtintion;
                }

            }
        }

        private string _DestinationFilePath;
        public const string DefaultExtintion = ".GZip";
        public CompressionLevel CompressionLevel { get; set; }
        public string CurrentFile { get; private set; }
    }
}