using static Kemorave.IO.File;

namespace Kemorave.IO
{
    [System.Serializable]
    public abstract class FileItemBase
    {
        public FileItemBase()
        {
        }
        private Category? fileCategory;

        public virtual bool FileExist()
        {
            return IO.File.FileEntryExist(this.Path);
        }
        protected virtual void SetPath(string path)
        {
            Path = path;
            _Name = IO.Path.GetFileName(Path);
        }
        // Exceptions:
        //   T:System.IO.IOException:
        //     The file already exists.
        //
        //   T:System.ArgumentException:
        //     /// newName is a zero-length string, contains only white
        //     space, or contains invalid characters as defined in System.IO.Path.InvalidPathChars.
        //
        //   T:System.UnauthorizedAccessException:
        //     The caller does not have the required permission.
        //
        //   T:System.IO.PathTooLongException:
        //     The specified path, file name, or both exceed the system-defined maximum length.
        //     For example, on Windows-based platforms, paths must be less than 248 characters,
        //     and file names must be less than 260 characters.
        //
        //   T:System.IO.DirectoryNotFoundException:
        //     The path specified in sourceFileName or destFileName is invalid, (for example,
        //     it is on an unmapped drive).
        //
        //   T:System.NotSupportedException:
        //     /// newName is in an invalid format.

        public virtual void Rename(string newName)
        {
            if (string.IsNullOrEmpty(newName))
            {
                throw new System.ArgumentException("Name can't be empty", nameof(newName));
            }
            newName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path), newName);
            System.IO.File.Move(Path, newName);
            SetPath(newName);
        }
        [System.Xml.Serialization.XmlIgnore]
        public virtual Category FileCategory
        {
            get
            {
                if (Type == FileType.File)
                    if (fileCategory == null)
                    {
                        fileCategory = File.GetFileCategory(Kemorave.IO.Path.GetFileExtension(this.Path));
                    }
                return fileCategory ?? Category.None;
            }
           protected set { fileCategory = value; }
        } 
        public virtual FileType Type { get; protected set; }
        public virtual bool IsMediaDeviceFile { get; protected set; } 
        public virtual long Size { get; protected set; }
        string _Name;
        public virtual string Name { get => _Name; set { Rename(value); } }
        public virtual string Path { get;private set; }  
        public override string ToString()
        {
            return Name;
        } 
    }
}