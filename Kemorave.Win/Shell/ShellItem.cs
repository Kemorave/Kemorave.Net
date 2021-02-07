using System;
using Microsoft.WindowsAPICodePack.Shell;

namespace Kemorave.Win.Shell
{
    public class ShellItem : IDisposable
    {
        private ShellItemThumbnail thumbnail;

        public ShellItem(string path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            ShellInfo = ShellObject.FromParsingName(path) ?? throw new ArgumentException(nameof(ShellInfo));
        }
        ~ShellItem()
        {
            Dispose(true);
        }
        private ShellItemThumbnail GetThumbnail()
        {
            if (thumbnail == null)
            {
                thumbnail = new ShellItemThumbnail(ShellInfo.Thumbnail);
            }
            return thumbnail;
        }

        protected virtual void Dispose(bool finalizer)
        {
            if (!finalizer)
            {
                System.GC.SuppressFinalize(this);
            }
            ShellInfo.Dispose();
        }
        public virtual void Dispose()
        {
            Dispose(false);
        }
        public virtual string Path { get; }
        public virtual ShellObject ShellInfo { get; }
        public virtual ShellItemThumbnail Thumbnail => GetThumbnail();
    }
}