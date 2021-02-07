using System;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.WindowsAPICodePack.Shell;

namespace Kemorave.Win.IO
{
    public static class ImageHelper
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject); 
        public static string DummyFilesFolderPath { get; set; } = Environment.CurrentDirectory + "\\Dummy";
         
        public static void SaveBitmapToPng(this BitmapSource bitmapSource, string fileName)
        {
            SaveBitmapToImage(bitmapSource, fileName, new PngBitmapEncoder(), ".Png");
        }
        public static void SaveBitmapToJpg(this BitmapSource bitmapSource, string fileName)
        {
            SaveBitmapToImage(bitmapSource, fileName, new JpegBitmapEncoder(), ".Jpeg");
        }
        public static void SaveBitmapToImage(this BitmapSource bitmapSource, string fileName, BitmapEncoder encoder, string Extension)
        {
            if (bitmapSource == null || string.IsNullOrEmpty(fileName))
            {
                return;
            }
            if (!fileName.EndsWith(Extension))
            {
                fileName += Extension;
            }
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource, null, null, null));
                encoder.Save(fs);
                encoder.Frames.Clear();
            }

        }



        public static BitmapSource GetImageFileThumbnail(string ImagePath, int decodeHeight = 0, int decodeWidth = 0)
        {
            using (System.IO.FileStream memory = new System.IO.FileStream(ImagePath, FileMode.Open, FileAccess.Read))
            {
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                if (decodeHeight > 0)
                {
                    bitmapImage.DecodePixelHeight = decodeHeight;
                }

                if (decodeWidth > 0)
                {
                    bitmapImage.DecodePixelWidth = decodeWidth;
                }

                bitmapImage.StreamSource = memory;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }
        public static BitmapSource GetShellObjectThumbnail(string name, ImageSize size, ShellThumbnailFormatOption Format)
        {
            using (ShellObject ShellObject = Microsoft.WindowsAPICodePack.Shell.ShellObject.FromParsingName(name))
            {
                ShellObject.Thumbnail.FormatOption = Format;
                switch (Format)
                {
                    case ShellThumbnailFormatOption.IconOnly:
                        {
                            switch (size)
                            {
                                case ImageSize.Default:
                                    return UnManagedBitmapToBitmapImage(ShellObject.Thumbnail.Bitmap, true);
                                case ImageSize.Small:
                                    return UnManagedBitmapToBitmapImage(ShellObject.Thumbnail.SmallBitmap, true);
                                case ImageSize.Medium:
                                    return UnManagedBitmapToBitmapImage(ShellObject.Thumbnail.MediumBitmap, true);
                                case ImageSize.Big:
                                    return UnManagedBitmapToBitmapImage(ShellObject.Thumbnail.LargeBitmap, true);
                                case ImageSize.Extra:
                                    return UnManagedBitmapToBitmapImage(ShellObject.Thumbnail.ExtraLargeBitmap, true);
                                default:
                                    break;
                            }
                        }
                        break;
                    default:
                        {
                            BitmapSource source = null;
                            switch (size)
                            {
                                case ImageSize.Default:
                                    source = ShellObject.Thumbnail.BitmapSource;
                                    break;
                                case ImageSize.Small:
                                    source = ShellObject.Thumbnail.SmallBitmapSource;
                                    break;
                                case ImageSize.Medium:
                                    source = ShellObject.Thumbnail.MediumBitmapSource;
                                    break;
                                case ImageSize.Big:
                                    source = ShellObject.Thumbnail.LargeBitmapSource;
                                    break;
                                case ImageSize.Extra:
                                    source = ShellObject.Thumbnail.ExtraLargeBitmapSource;
                                    break;
                                default:
                                    break;
                            }
                            source.Freeze();
                            return source;
                        }
                }
            }
            return null;
        }



        public static System.Windows.Media.Color GetImageMajorColor(BitmapSource image)
        {
            // Color color = new Color();
            int red = 0, green = 0, blue = 0, alpha = 0;


            // BitmapSource image = (BitmapSource)image;

            using (System.IO.MemoryStream fs = new MemoryStream())
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image, null, null, null));
                encoder.Save(fs);
                fs.Position = 0;
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(fs, false);
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        for (int y = 0; y < bitmap.Height; y++)
                        {
                            System.Drawing.Color color = bitmap.GetPixel(x, y);
                            alpha += color.A;
                            green += color.G;
                            blue += color.B;
                            red += color.R;
                        }
                    }
                    Func<int, int> avg = c => c / (int)(image.Width * image.Height);
                    red = avg(red);
                    green = avg(green);
                    alpha = avg(alpha);
                    blue = avg(blue);

                    if (red > 255)
                    {
                        red = 255;
                    }
                    if (blue > 255)
                    {
                        blue = 255;
                    }
                    if (green > 255)
                    {
                        green = 255;
                    }
                    if (alpha > 255)
                    {
                        alpha = 255;
                    }

                    if (red < 0)
                    {
                        red = 0;
                    }
                    if (blue < 0)
                    {
                        blue = 0;
                    }
                    if (green < 0)
                    {
                        green = 0;
                    }
                    if (alpha < 0)
                    {
                        alpha = 0;
                    }
                    System.Drawing.Color tempc = System.Drawing.Color.FromArgb(alpha, red, green, blue);
                    return System.Windows.Media.Color.FromArgb(tempc.A, tempc.R, tempc.G, tempc.B);
                }
            }
        }

        public static BitmapSource UnManagedBitmapToBitmapImage(Bitmap bitmap, bool MakeTransparent)
        {
            if (bitmap == null)
            {
                return null;
            }
            if (MakeTransparent)
            {
                bitmap.MakeTransparent(System.Drawing.Color.Black);
            }

            IntPtr hBitmap = bitmap.GetHbitmap(System.Drawing.Color.Transparent);
            BitmapSource retval;
            try
            {
                retval = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                             hBitmap,
                             IntPtr.Zero,
                             System.Windows.Int32Rect.Empty,
                             BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap);
            }
            retval.Freeze();

            return retval;
        }
        public static BitmapSource ManagedBitmapToBitmapImage(Bitmap bitmap, bool MakeTransparent)
        {
            if (bitmap == null)
            {
                return null;
            }
            if (MakeTransparent)
            {
                bitmap.MakeTransparent(System.Drawing.Color.Black);
            }

            using (System.IO.MemoryStream memory = new System.IO.MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                bitmap.Dispose();
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();

                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memory;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage as BitmapSource;
            }

        }
        public static ImageSource ToImageSource(this Icon icon)
        {
            ImageSource imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                System.Windows.Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return imageSource;
        }
        public static BitmapSource GetAssociatedIcon(string filePath, bool forExtension = false)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return null;
            }
            try
            {
                if (forExtension)
                {
                    return UnManagedBitmapToBitmapImage(Icon.ExtractAssociatedIcon(CreateTempraryFile(System.IO.Path.GetExtension(filePath))).ToBitmap(), false);
                }
                else
                {
                    return UnManagedBitmapToBitmapImage(Icon.ExtractAssociatedIcon(filePath).ToBitmap(), false);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string CreateTempraryFile(string Extension)
        {
            CheckTempraryFilesFolder();
            string path = DummyFilesFolderPath + "\\Temprary" + Extension;
            using (System.IO.File.Create(path))
            {
                ;
            }

            return path;
        }

        private static void CheckTempraryFilesFolder()
        {
            if (!Directory.Exists(DummyFilesFolderPath))
            {
                Directory.CreateDirectory(DummyFilesFolderPath);
            }
        }

        public static BitmapSource GetMediaFileThumbnail(string file, ImageSize size, FileType type)
        {
            try
            {
                string Extension = System.IO.Path.GetExtension(file);
                switch (type)
                {
                    case FileType.None:
                        file = CreateTempraryFile(".unkown");
                        break;
                    case FileType.File:
                        file = CreateTempraryFile(Extension);
                        break;
                    case FileType.Directory:
                        System.IO.Directory.CreateDirectory(DummyFilesFolderPath);
                        file = DummyFilesFolderPath;
                        break;
                    case FileType.Drive:
                        file = CreateTempraryFile(".vhdx");
                        break;
                    case FileType.Device:
                        file = @"C:\Windows\System32\hdwwiz.exe";
                        break;
                    default:
                        if (string.IsNullOrEmpty(Extension) || Extension == ".")
                        {
                            file = CreateTempraryFile(".unkown");
                        }
                        file = CreateTempraryFile(Extension);
                        break;
                }
                return GetShellObjectThumbnail(file, size, ShellThumbnailFormatOption.IconOnly);

            }
            catch (Exception)
            {

            }
            return null;
        }
    }
}
public enum FileType
{
    None,
    File,
    Directory,
    Drive,
    Device
}
public enum ImageSize
{
    Default,
    Small,
    Medium,
    Big,
    Extra
}
