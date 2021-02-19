
using System;
using Microsoft.WindowsAPICodePack.Shell;

namespace Kemorave.Win.IO
{
    public class DriveInfo : DirectoryInfo
    {
        private readonly System.IO.DriveInfo SystemInfo;
        public DriveInfo(System.IO.DriveInfo systemInfo) : base(systemInfo.Name)
        {
            SystemInfo = systemInfo ?? throw new ArgumentNullException(nameof(systemInfo));
        }
        public DriveInfo(string path) : base(path)
        {
            SystemInfo = new System.IO.DriveInfo(path);
        }
        public DriveInfo(ShellObject shellInfo) : base(shellInfo)
        {
            SystemInfo = new System.IO.DriveInfo(shellInfo.Name);
        }


        public long? FreeSpace =>  SystemInfo?.AvailableFreeSpace;

        public long? UsedSpace =>  TotalSize - FreeSpace;

        public long? TotalSize =>  SystemInfo?.TotalSize;



    }
}