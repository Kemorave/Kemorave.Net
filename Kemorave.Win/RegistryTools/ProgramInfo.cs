using System;
using System.Linq;
using System.Windows.Media;
using Kemorave.Win.IO;
using Microsoft.Win32;

namespace Kemorave.Win.RegistryTools
{
    public sealed class ProgramInfo : RegistryItemBase
    {
        public bool ShellThumbnailLoaded, ShellIconLoaded;

        private ProgramInfo()
        {
        }
        public static ProgramInfo ToProgramInfo(RegistryKey appRegistryKey)
        {
            if (appRegistryKey == null)
            {
                throw new ArgumentNullException(nameof(appRegistryKey));
            }

            DateTime InstallDate = DateTime.MinValue;
            //  string tmp;
            ProgramInfo pr = new ProgramInfo()
            {
                Comments = appRegistryKey.GetValue("Comments")?.ToString(),
                Version = new Version(((appRegistryKey.GetValue("Version") as int?) ?? 0), 0),
                IsSystemComponent = appRegistryKey.GetValue("SystemComponent") as int? == 1 ? true : false,
                DisplayVersion = appRegistryKey.GetValue("DisplayVersion")?.ToString(),
                InstallLocation = appRegistryKey.GetValue("InstallLocation")?.ToString(),
                Publisher = appRegistryKey.GetValue("Publisher")?.ToString(),
                HelpLink = appRegistryKey.GetValue("HelpLink")?.ToString(),
                HelpFile = appRegistryKey.GetValue("URLInfoAbout")?.ToString(),
                UpdateLink = appRegistryKey.GetValue("URLUpdateInfo")?.ToString(),
                ModifyPath = appRegistryKey.GetValue("ModifyPath")?.ToString(),
                DisplayName = appRegistryKey.GetValue("DisplayName")?.ToString(),
                InstallDate = DateTime.TryParse(appRegistryKey.GetValue("InstallDate")?.ToString()?.Insert(4, "/")?.Insert(7, "/"), out InstallDate) ? InstallDate : DateTime.MinValue,
                DisplayInstallDate = InstallDate == DateTime.MinValue ? appRegistryKey.GetValue("InstallDate")?.ToString() : InstallDate.ToShortDateString(),
                AboutLink = appRegistryKey.GetValue("URLInfoAbout")?.ToString(),
                UninstallString = appRegistryKey.GetValue("UninstallString")?.ToString(),
                QuietUninstallString = appRegistryKey.GetValue("QuietUninstallString")?.ToString(),
                EstimatedSize = long.TryParse(appRegistryKey.GetValue("EstimatedSize")?.ToString(), out long EstimatedSize) ? EstimatedSize * 1024 : 0,
                IconPath = appRegistryKey.GetValue("DisplayIcon")?.ToString()
            };
            // System.Diagnostics.Debug.WriteLine(tmp);
            if (string.IsNullOrEmpty(pr.HelpFile))
            {
                pr.HelpFile = null;
            }
            if (string.IsNullOrEmpty(pr.AboutLink))
            {
                pr.AboutLink = null;
            }
            if (string.IsNullOrEmpty(pr.HelpLink))
            {
                pr.HelpLink = null;
            }
            if (string.IsNullOrEmpty(pr.UpdateLink))
            {
                pr.UpdateLink = null;
            }
            if (string.IsNullOrEmpty(pr.Publisher))
            {
                pr.Publisher = null;
            }
            if (string.IsNullOrEmpty(pr.DisplayVersion))
            {
                pr.DisplayVersion = pr.Version?.ToString();
            }
            if (string.IsNullOrEmpty(pr.DisplayName))
            {
                pr.DisplayName = null;
            }
            if (string.IsNullOrEmpty(pr.Comments))
            {
                pr.Comments = null;
            }
            if (string.IsNullOrEmpty(pr.ModifyPath))
            {
                pr.ModifyPath = null;
            }
            if (string.IsNullOrEmpty(pr.DisplayInstallDate))
            {
                pr.DisplayInstallDate = null;
            }
            if ((pr.InstallDate == DateTime.MinValue))
            {
                pr.InstallDate = null;
            }

            if ((pr.EstimatedSize <= 0))
            {
                pr.EstimatedSize = null;
            }
            return pr;
        }


        private ImageSource GetIcon()
        {
            if (!string.IsNullOrEmpty(IconPath))
            {
                if (IconPath.Contains(',') && !System.IO.File.Exists(IconPath))
                {
                    foreach (string item in IconPath.Split(','))
                    {
                        if (System.IO.File.Exists(item))
                        {
                            return ImageHelper.GetAssociatedIcon(item);
                        }
                    }
                }
                else
                {
                    return ImageHelper.GetAssociatedIcon(IconPath);
                }
            }
            return ImageHelper.GetAssociatedIcon("Hola.exe", true);
        }
        public long? SizeOnDisk => GetSizeOnDisk();

        private long? _SizeOnDisk;
        private long? GetSizeOnDisk()
        {
            if (!string.IsNullOrEmpty(InstallLocation))
            {
                InstallLocation = System.IO.Path.GetFullPath(InstallLocation);
                if (_SizeOnDisk == null)
                {
                    _SizeOnDisk = 0;
                    _SizeOnDisk = CalculateDirSize(InstallLocation);
                }
            }
            return _SizeOnDisk;
        }

        private long CalculateDirSize(string installLocation)
        {
            long size = 0;
            foreach (string item in System.IO.Directory.EnumerateDirectories(installLocation))
            {
                size += CalculateDirSize(item);
            }
            foreach (string item in System.IO.Directory.EnumerateFiles(installLocation))
            {
                try
                {
                    size += new System.IO.FileInfo(item).Length;
                }
                catch { }
            }
            return size;
        }

        public ImageSource AssociatedIcon => GetIcon();
        public bool IsSystemComponent { get; private set; }
        public string IconPath { get; private set; }
        public string AboutLink { get; private set; }
        public long? EstimatedSize { get; private set; }
        public string Comments { get; private set; }
        public string UninstallString { get; private set; }
        public string QuietUninstallString { get; private set; }
        public Version Version { get; private set; }
        public string DisplayVersion { get; private set; }
        public string Publisher { get; private set; }
        public string ModifyPath { get; private set; }
        public string InstallLocation { get; private set; }
        public string UpdateLink { get; private set; }
        public string HelpLink { get; private set; }
        public string HelpFile { get; private set; }
        public DateTime? InstallDate { get; private set; }
        public string DisplayInstallDate { get; private set; }
        public override string ToString()
        {
            return string.IsNullOrEmpty(DisplayName) ? System.IO.Path.GetFileName(this.RegistryKey) : DisplayName;
        }
    }
}