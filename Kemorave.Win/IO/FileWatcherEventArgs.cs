using System;
using System.IO;

namespace Kemorave.Win.IO
{
 public class FileWatcherEventArgs : EventArgs
 {
        private FileSystemEventArgs args;

        public FileWatcherEventArgs(FileSystemEventArgs args)
        {
            this.args = args;
        }

        public FileWatcherEventArgs(FileSystemEventArgs fileChangeArgs, RenamedEventArgs fileRenameArgs)
        {
            FileChangeArgs = fileChangeArgs;
            FileRenameArgs = fileRenameArgs;
        }

        public System.IO.FileSystemEventArgs FileChangeArgs { get;  }
  public System.IO.RenamedEventArgs FileRenameArgs { get; }
    }
}
