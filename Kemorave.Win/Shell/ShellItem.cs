using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAPICodePack.Shell;

namespace Kemorave.Win.Shell
{
    public class ShellItem : IDisposable
    {
        private ShellItemThumbnail _Thumbnail;
        private ShellObject _ShellInfo;
        public ShellItem(string path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
        }
        public ShellItem(ShellObject shellInfo)
        {
            _ShellInfo = shellInfo ?? throw new ArgumentNullException(nameof(shellInfo));
            Path = shellInfo.ParsingName;
        }
        public static ShellItem FromParsingName(string path) => new ShellItem(ShellObject.FromParsingName(path));
        public static IEnumerable<ShellItem> FromContainer(ShellContainer container) => container.Select(s => new ShellItem(s));
        ~ShellItem()
        {
           // ShellInfo.Properties.DefaultPropertyCollection[0].Description.DisplayName
            Dispose(true);
        }
        public System.Windows.Media.ImageSource AssociatedIcon { get =>Kemorave.Win.IO.ImageHelper.ToImageSource( System.Drawing.Icon.ExtractAssociatedIcon(Path)); }
        private ShellItemThumbnail GetThumbnail()
        {
            if (_Thumbnail == null)
            {
                _Thumbnail = new ShellItemThumbnail(ShellInfo.Thumbnail);
            }
            return _Thumbnail;
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
        public string Name => ShellInfo?.Name;
        public virtual string Path { get; }
      //  public string ObjectType => ShellInfo?.Properties.System..ValueAsObject?.ToString();
        public virtual ShellObject ShellInfo
        {
            get
            {
                if (_ShellInfo == null)
                {
                    _ShellInfo = ShellObject.FromParsingName(Path) ?? throw new ArgumentException(nameof(ShellInfo));
                }
                return _ShellInfo;
            }
        }
        public virtual ShellItemThumbnail Thumbnail => GetThumbnail();
    }
}