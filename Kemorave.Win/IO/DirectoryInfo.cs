using System.Collections.Generic; 

namespace Kemorave.Win.IO
{
    public class DirectoryInfo : Shell.ShellItem
    {
        public DirectoryInfo(string path) : base(path)
        {
        }

        public DirectoryInfo(Microsoft.WindowsAPICodePack.Shell.ShellObject shellInfo) : base(shellInfo)
        {
        }

        public IEnumerable<Microsoft.WindowsAPICodePack.Shell.ShellObject> ShellObjects => ShellInfo as Microsoft.WindowsAPICodePack.Shell.ShellFolder;
    }
}