using System.Windows.Media;
using Kemorave.Win.IO;

namespace Kemorave.Win.Shell
{
    /// <summary>
    /// Thumnail size defn
    /// </summary>
    public enum ThumbnailSize
    {
        Default,
        //
        // Summary:
        //     Gets the extra-large size property for a 1024x1024 pixel Shell Thumbnail.
        ExtraLarge,
        //
        // Summary:
        //     Gets the large size property for a 256x256 pixel Shell Thumbnail.
        Large,
        //
        // Summary:
        //     Maximum size for the Shell Thumbnail, 1024x1024 pixels.
        Maximum,
        //
        // Summary:
        //     Gets the medium size property for a 96x96 pixel Shell Thumbnail.
        Medium,
        //
        // Summary:
        //     Gets the small size property for a 32x32 pixel Shell Thumbnail.
        Small
    }

    public class ShellItemThumbnail
    {
        private readonly string Path;
        private readonly Microsoft.WindowsAPICodePack.Shell.ShellThumbnail ShellThumbnail;
        internal ShellItemThumbnail(Microsoft.WindowsAPICodePack.Shell.ShellThumbnail thumbnail)
        {
            ShellThumbnail = thumbnail;
        }
        public ImageSource DefaultThumbnail => GetShellThumbnail(ThumbnailSize.Default);
        public ImageSource SmallThumbnail => GetShellThumbnail(ThumbnailSize.Small);
        public ImageSource MediumThumbnail => GetShellThumbnail(ThumbnailSize.Medium);
        public ImageSource LargeThumbnail => GetShellThumbnail(ThumbnailSize.Large);
        public ImageSource ExtraLargeThumbnail => GetShellThumbnail(ThumbnailSize.ExtraLarge);
        public ImageSource MaximumThumbnail => GetShellThumbnail(ThumbnailSize.Maximum);

        public ImageSource DefaultIcon => GetShellIcon(ThumbnailSize.Default);
        public ImageSource SmallIcon => GetShellIcon(ThumbnailSize.Small);
        public ImageSource MediumIcon => GetShellIcon(ThumbnailSize.Medium);
        public ImageSource LargeIcon => GetShellIcon(ThumbnailSize.Large);
        public ImageSource ExtraLargeIcon => GetShellIcon(ThumbnailSize.ExtraLarge);
        public ImageSource MaximumIcon => GetShellIcon(ThumbnailSize.Maximum);

        private ImageSource GetShellThumbnail(ThumbnailSize size)
        {
            ImageSource source = null;
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
        private ImageSource GetShellIcon(ThumbnailSize size)
        {
            ImageSource source = null;
            System.Drawing.Icon icon = null;
            switch (size)
            {
                case ThumbnailSize.ExtraLarge:
                    icon = ShellThumbnail.SmallIcon; break;
                case ThumbnailSize.Large:
                    icon = ShellThumbnail.LargeIcon; break;
                case ThumbnailSize.Maximum:
                    ShellThumbnail.AllowBiggerSize = true;
                    ShellThumbnail.CurrentSize = Microsoft.WindowsAPICodePack.Shell.DefaultThumbnailSize.Maximum;
                    icon = ShellThumbnail.Icon; break;
                case ThumbnailSize.Medium:
                    icon = ShellThumbnail.MediumIcon; break;
                case ThumbnailSize.Small:
                    icon = ShellThumbnail.SmallIcon; break;
                default:
                    icon = ShellThumbnail.Icon; break; 
            }
            using (icon)
            {
                source = icon.ToImageSource();
                source?.Freeze();
            }
            return source;
        }
    }
}