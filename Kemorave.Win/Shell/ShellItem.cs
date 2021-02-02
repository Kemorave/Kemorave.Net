using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.WindowsAPICodePack.Shell;

namespace Kemorave.Win.Shell
{
    public class ShellItem : IDisposable
    {
        private ShellItemThumbnail thumbnail;

        ~ShellItem()
        {

        }
        public virtual string Path { get; }
        public ShellObject ShellInfo { get; }
        public ShellItemThumbnail Thumbnail => GetThumbnail();
        private ShellItemThumbnail GetThumbnail()
        {
            if (thumbnail == null)
            {
                thumbnail = new ShellItemThumbnail(ShellInfo.Thumbnail);
            }
            return thumbnail;
        }

        public void Dispose(bool finalizer)
        {
            if (!finalizer)
            {
                System.GC.SuppressFinalize(this);
            }
            ShellInfo.Dispose();
        }
        public void Dispose()
        {
            Dispose(false);
        }
    }
}