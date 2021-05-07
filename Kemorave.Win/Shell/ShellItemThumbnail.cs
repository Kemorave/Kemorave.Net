using System.Diagnostics;
using System.Windows.Media;

namespace Kemorave.Win.Shell
{

    public class ShellItemThumbnail
    {
        private readonly Microsoft.WindowsAPICodePack.Shell.ShellThumbnail ShellThumbnail;
        internal ShellItemThumbnail(Microsoft.WindowsAPICodePack.Shell.ShellThumbnail thumbnail)
        {
            ShellThumbnail = thumbnail;
        }
        /// <summary>
        /// Thumbnail or icon for object (most familiar to users)
        /// </summary>
        public ImageSource Default => GetDefaultShellThumbnail(ThumbnailSize.Default);
        public ImageSource Small => GetDefaultShellThumbnail(ThumbnailSize.Small);
        public ImageSource Medium=> GetDefaultShellThumbnail(ThumbnailSize.Medium);
        public ImageSource Large => GetDefaultShellThumbnail(ThumbnailSize.Large);
        public ImageSource ExtraLarge => GetDefaultShellThumbnail(ThumbnailSize.ExtraLarge);
        public ImageSource Maximum => GetDefaultShellThumbnail(ThumbnailSize.Maximum);

        /// <summary>
        /// Thumnail only 
        /// </summary>
        public ImageSource DefaultThumbnail => GetShellThumbnail(ThumbnailSize.Default);
        public ImageSource SmallThumbnail => GetShellThumbnail(ThumbnailSize.Small);
        public ImageSource MediumThumbnail => GetShellThumbnail(ThumbnailSize.Medium);
        public ImageSource LargeThumbnail => GetShellThumbnail(ThumbnailSize.Large);
        public ImageSource ExtraLargeThumbnail => GetShellThumbnail(ThumbnailSize.ExtraLarge);
        public ImageSource MaximumThumbnail => GetShellThumbnail(ThumbnailSize.Maximum);
        /// <summary>
        /// Icon only
        /// </summary>
        public ImageSource DefaultIcon => GetShellIcon(ThumbnailSize.Default);
        public ImageSource SmallIcon => GetShellIcon(ThumbnailSize.Small);
        public ImageSource MediumIcon => GetShellIcon(ThumbnailSize.Medium);
        public ImageSource LargeIcon => GetShellIcon(ThumbnailSize.Large);
        public ImageSource ExtraLargeIcon => GetShellIcon(ThumbnailSize.ExtraLarge);
        public ImageSource MaximumIcon => GetShellIcon(ThumbnailSize.Maximum);
        private ImageSource GetDefaultShellThumbnail(ThumbnailSize size)
        {
            if (ShellThumbnail.FormatOption != Microsoft.WindowsAPICodePack.Shell.ShellThumbnailFormatOption.Default)
            {
                Debug.WriteLine("Set default has happened ! :o");
                ShellThumbnail.FormatOption = Microsoft.WindowsAPICodePack.Shell.ShellThumbnailFormatOption.Default;
            }
            return GetImageSource(size);
        }
        private ImageSource GetShellThumbnail(ThumbnailSize size)
        {
            if (ShellThumbnail.FormatOption != Microsoft.WindowsAPICodePack.Shell.ShellThumbnailFormatOption.ThumbnailOnly)
            {
                ShellThumbnail.FormatOption = Microsoft.WindowsAPICodePack.Shell.ShellThumbnailFormatOption.ThumbnailOnly;
            }
            return GetImageSource(size);
        }
        private ImageSource GetShellIcon(ThumbnailSize size)
        {
            if (ShellThumbnail.FormatOption != Microsoft.WindowsAPICodePack.Shell.ShellThumbnailFormatOption.IconOnly)
            {
                ShellThumbnail.FormatOption = Microsoft.WindowsAPICodePack.Shell.ShellThumbnailFormatOption.IconOnly;
            }
            return GetImageSource(size);
        }

        private ImageSource GetImageSource(ThumbnailSize size)
        {
            ImageSource source;
            switch (size)
            {
                case ThumbnailSize.ExtraLarge:
                    source = ShellThumbnail.ExtraLargeBitmapSource; break;
                case ThumbnailSize.Large:
                    source = ShellThumbnail.LargeBitmapSource; break;
                case ThumbnailSize.Maximum:
                    ShellThumbnail.AllowBiggerSize = true;
                    ShellThumbnail.CurrentSize = Microsoft.WindowsAPICodePack.Shell.DefaultThumbnailSize.Maximum;
                    source = ShellThumbnail.BitmapSource; break;
                case ThumbnailSize.Medium:
                    source = ShellThumbnail.MediumBitmapSource; break;
                case ThumbnailSize.Small:
                    source = ShellThumbnail.SmallBitmapSource; break;
                default:
                    source = ShellThumbnail.BitmapSource; break;
            }
            source?.Freeze();
            return source;
        }

    }
}