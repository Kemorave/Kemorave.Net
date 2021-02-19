using System;
using System.IO;

namespace Kemorave.Win.IO
{
    public sealed class FileWatcherSettings
    {
        public FileWatcherSettings()
        {
            // System.IO.FileSystemWatcher
            Filter = "*.*";
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite;
            WatcherChangeTypes = WatcherChangeTypes.All;
            EnableRaisingEvents = true;
            InternalBufferSize = 8192;
        }

        public FileWatcherSettings(int internalBufferSize, string filter, bool includeSubdirectories)
        {
            InternalBufferSize = internalBufferSize;
            Filter = filter ?? throw new ArgumentNullException(nameof(filter));
            IncludeSubdirectories = includeSubdirectories;
        }

        public FileWatcherSettings(int internalBufferSize, string filter, bool includeSubdirectories, int timeoutMilliseconds, WatcherChangeTypes watcherChangeTypes, NotifyFilters notifyFilter) : this(internalBufferSize, filter, includeSubdirectories)
        {
            TimeoutMilliseconds = timeoutMilliseconds;
            WatcherChangeTypes = watcherChangeTypes;
            NotifyFilter = notifyFilter;
        }

        public static FileWatcherSettings Default { get; } = new FileWatcherSettings();
        public System.IO.WatcherChangeTypes WatcherChangeTypes { get; set; }
        public System.ComponentModel.ISite Site { get; set; }
        public int InternalBufferSize { get; set; }
        public int TimeoutMilliseconds { get; set; }
        public string Filter { get; set; }
        public bool EnableRaisingEvents { get; set; }
        public bool IncludeSubdirectories { get; set; }
        public System.IO.NotifyFilters NotifyFilter { get; set; }
        public System.ComponentModel.ISynchronizeInvoke SynchronizeInvoke { get; set; }
    }
}