using System;
using System.Linq;
using System.Windows.Media;
using Kemorave.Win.IO;
using Microsoft.Win32;

namespace Kemorave.Win.RegistryTools
{
    public class ProgramInfo : RegistryItemBase
    {
        public bool ShellThumbnailLoaded, ShellIconLoaded;

        private ProgramInfo()
        {
        }
        internal static ProgramInfo ToProgramInfo(RegistryKey appRegistryKey)
        {
            DateTime InstallDate = DateTime.MinValue;
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
                InstallDate = DateTime.TryParse(appRegistryKey.GetValue("InstallDate")?.ToString(), out InstallDate) ? InstallDate : DateTime.MinValue,
                DisplayInstallDate = InstallDate == DateTime.MinValue ? appRegistryKey.GetValue("InstallDate")?.ToString() : InstallDate.ToLongTimeString(),
                AboutLink = appRegistryKey.GetValue("URLInfoAbout")?.ToString(),
                UninstallString = appRegistryKey.GetValue("UninstallString")?.ToString(),
                QuietUninstallString = appRegistryKey.GetValue("QuietUninstallString")?.ToString(),
                EstimatedSize = long.TryParse(appRegistryKey.GetValue("EstimatedSize")?.ToString(), out long EstimatedSize) ? EstimatedSize * 1024 : 0,
                IconPath = appRegistryKey.GetValue("DisplayIcon")?.ToString()
            };
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


        protected ImageSource GetIcon()
        {
            if (!string.IsNullOrEmpty(IconPath))
            {
                if (IconPath.Contains(',') && !System.IO.File.Exists(IconPath))
                {
                    foreach (string item in IconPath.Split(','))
                    {
                        if (System.IO.File.Exists(item))
                        {
                            return ImageHelper.GetShellObjectThumbnail(item, ImageSize.Default, Microsoft.WindowsAPICodePack.Shell.ShellThumbnailFormatOption.Default);
                        }
                    }
                }
                else
                {
                    return ImageHelper.GetShellObjectThumbnail(IconPath, ImageSize.Default, Microsoft.WindowsAPICodePack.Shell.ShellThumbnailFormatOption.Default);
                }
            }
            return ImageHelper.GetMediaFileThumbnail(IconPath, ImageSize.Default, FileType.File);
        }

        public virtual ImageSource AssociatedIcon => GetIcon();
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