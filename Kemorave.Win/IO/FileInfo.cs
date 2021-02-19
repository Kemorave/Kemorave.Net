using System;
using System.ComponentModel;
using System.IO;
using Microsoft.WindowsAPICodePack.Shell;

namespace Kemorave.Win.IO
{

    public class FileInfo : Shell.ShellItem
    {
        public FileInfo(string path) : base(path)
        {
        }

        public FileInfo(ShellObject shellInfo) : base(shellInfo)
        {
        }
    }
}