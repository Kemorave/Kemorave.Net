using System.Linq;
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

        public abstract bool FileExist();
        public abstract void SetPath(string path);
        public abstract void Rename(string newName);

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
        public virtual bool IsMediaDeviceFile { get; set; } 
        public abstract long Size { get; } 
        public abstract string Name { get; set; } 
        public virtual string Path { get; set; }  
        public abstract bool? IsAvailable { get; } 
        public override string ToString()
        {
            return Name;
        } 
    }
}