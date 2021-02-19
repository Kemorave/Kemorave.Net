using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kemorave.Win.IO
{
    public class MultiFileWatcher : IDisposable
    {
        public MultiFileWatcher()
        {
            UpdateTimeout = 0;
            WatchersList = new List<System.IO.FileSystemWatcher>();
        }
        ~MultiFileWatcher()
        {
            Dispose(true);
        }
        public event EventHandler<FileWatcherEventArgs> OnFileUpdate;
        private volatile bool isUpdateBinding = false;
        private string _lastUpdatedPath = string.Empty;
        private void Dispose(bool v)
        {
            if (!v)
            {
                System.GC.SuppressFinalize(this);
            }
            Dispose();
        }

        public async Task WatchFileAsync(string path, FileWatcherSettings settings)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("Path is null");
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (!System.IO.File.Exists(path) && !System.IO.Directory.Exists(path))
            {
                throw new System.IO.DirectoryNotFoundException("Directory or file not found");
            }
            CurrentObservedPath = path;
            if (this.IsOnWatchList(path))
            {
                CurrentObservedPath = path;
                return;
            }
            FileSystemWatcher Wtacher = null;
            await Task.Run(() =>
            {
                try
                {
                    using (Wtacher = AddPath(path, settings))
                    {
                        Wtacher.Renamed += (s, a) => { RiseEventChange(path, a, a); };
                        Wtacher.Deleted += (s, a) => { RiseEventChange(path, a, null); };
                        Wtacher.Created += (s, a) => { RiseEventChange(path, a, null); };
                        Wtacher.Disposed += (s, a) => { this.RemovePath(path); };
                        if (settings.TimeoutMilliseconds > 0)
                        {
                            Wtacher.WaitForChanged(System.IO.WatcherChangeTypes.All, settings.TimeoutMilliseconds);
                        }
                        else
                        {
                            Wtacher.WaitForChanged(System.IO.WatcherChangeTypes.All);
                        }
                    }
                    Debug.WriteLine("Watching over for file " + path);
                }
                catch (Exception) { }
            });
        }
        internal static System.IO.FileSystemWatcher GetWatcher(string path, FileWatcherSettings settings)
        {
            System.IO.FileSystemWatcher FileWatcher = new System.IO.FileSystemWatcher(path, settings.Filter);
            FileWatcher.BeginInit();
            FileWatcher.EnableRaisingEvents = settings.EnableRaisingEvents;
            if (settings.Site != null)
            {
                FileWatcher.Site = settings.Site;
            }

            if (settings.SynchronizeInvoke != null)
            {
                FileWatcher.SynchronizingObject = settings.SynchronizeInvoke;
            }

            FileWatcher.NotifyFilter = settings.NotifyFilter;
            if (settings.InternalBufferSize != 8192)
            {
                FileWatcher.InternalBufferSize = settings.InternalBufferSize;
            }

            FileWatcher.IncludeSubdirectories = settings.IncludeSubdirectories;
            FileWatcher.EndInit();
            return FileWatcher;
        }

        protected System.IO.FileSystemWatcher AddPath(string path, FileWatcherSettings settings)
        {
            if (IsOnWatchList(path))
            {
                return WatchersList.First(a => a.Path == path);
            }
            System.IO.FileSystemWatcher FileWatcher = GetWatcher(path, settings ?? new FileWatcherSettings());
            (WatchersList as List<System.IO.FileSystemWatcher>).Add(FileWatcher);
            IsWatching = true;
            return FileWatcher;
        }
        protected void RemovePath(string path)
        {
            (WatchersList as List<System.IO.FileSystemWatcher>).Remove(WatchersList.FirstOrDefault(wa => wa.Path == path));
            if (WatchersList.Count <= 0)
            {
                IsWatching = false;
            }
        }
        public bool IsOnWatchList(string path)
        {
            return WatchersList?.FirstOrDefault(wa => wa.Path == path) != null;
        }
        protected virtual void RiseEventChange(string path, System.IO.FileSystemEventArgs args, System.IO.RenamedEventArgs renameArgs)
        {
            Debug.WriteLine("Updating over " + path);

            if (path != CurrentObservedPath && WatchCurrentPathOnly)
            {
                return;
            }
            if (IsUpdateBinding)
            {
                return;
            }

            if (_lastUpdatedPath.Equals(path, StringComparison.Ordinal) || this.IsOnWatchList(path))
            {
                _lastUpdatedPath = path;
                if (UpdateTimeout > 0)
                {
                    IsUpdateBinding = true;
                    Task.Run(() =>
                   {
                       Thread.Sleep(UpdateTimeout * 1000);
                       IsUpdateBinding = false;
                       OnFileUpdate.Invoke(this, new FileWatcherEventArgs(args));
                   });
                }
                else
                {
                    OnFileUpdate.Invoke(this, new FileWatcherEventArgs(args, renameArgs));
                }
            }
        }

        public void Dispose()
        {
            foreach (FileSystemWatcher item in this.WatchersList)
            {
                item.Dispose();
            }
        }

        protected IReadOnlyList<System.IO.FileSystemWatcher> WatchersList { get; }
        public IEnumerable<string> WatchersPathList => WatchersList.Select(s => s.Path);
        public bool IsWatching { get; private set; }
        public bool WatchCurrentPathOnly { get; set; }
        public int UpdateTimeout { get; set; }
        public string CurrentObservedPath { get; protected set; }
        public bool IsUpdateBinding { get => isUpdateBinding; set => isUpdateBinding = value; }
    }
}