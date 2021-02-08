using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;
using Kemorave.Win.IO;
using Microsoft.Win32;

namespace Kemorave.Win.RegistryTools
{
    public class ProgramInfo : RegistryItemBase
    {
        public bool ShellThumbnailLoaded, ShellIconLoaded;
        private static readonly bool is64bit = true;

        private ProgramInfo()
        {
        }
        private static ProgramInfo ToProgramInfo(RegistryKey appRegistryKey)
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
        public static string GetCommandInCommaAndArgumentsOutsideByExe(string command)
        {
            int exeLocationIndex = command.IndexOf(".exe") + 4;
            string cmd = command.Substring(0, exeLocationIndex);
            string arguments = command.Substring(command.IndexOf(".exe") + 4, command.Length - exeLocationIndex);
            return "\"" + cmd + "\"" + arguments;
        }
        public static void UninstallApplication(string uninstallString)
        {
            if (string.IsNullOrEmpty(uninstallString))
            {
                throw new ArgumentNullException(nameof(uninstallString));
            }

            ProcessStartInfo startInfo = new ProcessStartInfo();

            int indexofexe = uninstallString.IndexOf(".exe");
            //Check for executable existence 
            if (indexofexe > 0)
            {
                uninstallString = uninstallString.Replace(@"""", string.Empty);

                //Get exe path 
                string uninstallerPath = uninstallString.Substring(0, indexofexe + 4);
                startInfo.FileName = uninstallerPath;

                //Check for arguments
                if (uninstallerPath.Length != uninstallString.Length)
                {
                    string args = uninstallString.Substring(uninstallerPath.Length);
                    if (!string.IsNullOrEmpty(args))
                    {

                        /*If not set to false You will get InvalidOperationException :
                         *The Process object must have the UseShellExecute property set to false in order to use environment variables.*/
                        startInfo.UseShellExecute = false;

                        startInfo.Arguments = args;
                    }
                }
            }
            //Not tested 
            else
            {
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "/c " + uninstallString;
            }

            //Start the process
            Process.Start(startInfo).WaitForExit(); 
        }
        public static bool IsApplicationInstalled(string displayName)
        {
            if (string.IsNullOrEmpty(displayName))
            {
                return false;
            }
            string tempName;
            RegistryKey key;

            // search in: CurrentUser
            using (key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"))
            {
                foreach (string keyName in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(keyName))
                    {
                        tempName = subkey.GetValue("DisplayName") as string;
                        if (displayName.Equals(tempName, StringComparison.OrdinalIgnoreCase) == true)
                        {
                            return true;
                        }

                    }
                }
            }

            using (key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"))
            {
                foreach (string keyName in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(keyName))
                    {
                        tempName = subkey.GetValue("DisplayName") as string;
                        if (displayName.Equals(tempName, StringComparison.OrdinalIgnoreCase) == true)
                        {
                            return true;
                        }

                    }
                }
            }

            using (key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall"))
            {
                foreach (string keyName in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(keyName))
                    {
                        tempName = subkey.GetValue("DisplayName") as string;
                        if (displayName.Equals(tempName, StringComparison.OrdinalIgnoreCase) == true)
                        {
                            return true;
                        }

                    }
                }
            }

            return false;
        }
        public static List<ProgramInfo> GetLocalMachineInstalledPrograms()
        {
            List<ProgramInfo> programsinfolist = new List<ProgramInfo>();
            try
            {
                using (RegistryKey uninstallRegistryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", RegistryKeyPermissionCheck.ReadSubTree, System.Security.AccessControl.RegistryRights.ReadKey))
                {
                    if (uninstallRegistryKey == null)
                    {
                        return programsinfolist;
                    }
                    else
                    {
                        foreach (string key in uninstallRegistryKey.GetSubKeyNames())
                        {
                            try
                            {
                                using (RegistryKey appRegistryKey = uninstallRegistryKey.OpenSubKey(key, RegistryKeyPermissionCheck.ReadSubTree, System.Security.AccessControl.RegistryRights.ReadKey))
                                {
                                    if (appRegistryKey == null)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        ProgramInfo tempInfo = ToProgramInfo(appRegistryKey);
                                        tempInfo.SetRegistryKey(System.IO.Path.Combine(uninstallRegistryKey.Name, key), RegistryHive.LocalMachine, tempInfo.DisplayName);
                                        programsinfolist.Add(tempInfo);
                                    }
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                }
                using (RegistryKey uninstallRegistryKey = Microsoft.Win32.RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, is64bit ? RegistryView.Registry64 : RegistryView.Registry32).OpenSubKey(@"SOFTWARE\WOW6432Node\\Microsoft\Windows\CurrentVersion\Uninstall", false))
                {
                    if (uninstallRegistryKey == null)
                    {
                        return programsinfolist;
                    }
                    else
                    {
                        foreach (string key in uninstallRegistryKey.GetSubKeyNames())
                        {
                            using (RegistryKey appRegistryKey = uninstallRegistryKey.OpenSubKey(key, RegistryKeyPermissionCheck.ReadSubTree, System.Security.AccessControl.RegistryRights.ReadKey))
                            {
                                if (appRegistryKey == null)
                                {
                                    continue;
                                }
                                else
                                {
                                    ProgramInfo tempInfo = ToProgramInfo(appRegistryKey);
                                    tempInfo.SetRegistryKey(System.IO.Path.Combine(uninstallRegistryKey.Name, key), RegistryHive.LocalMachine, tempInfo.DisplayName);
                                    programsinfolist.Add(tempInfo);

                                }
                            }
                        }
                    }
                }
                return programsinfolist;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static List<ProgramInfo> GetCurrentUserInstalledPrograms()
        {
            List<ProgramInfo> programsinfolist = new List<ProgramInfo>();
            try
            {
                using (RegistryKey uninstallRegistryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", RegistryKeyPermissionCheck.ReadSubTree, System.Security.AccessControl.RegistryRights.ReadKey))
                {
                    if (uninstallRegistryKey == null)
                    {
                        return programsinfolist;
                    }
                    else
                    {
                        foreach (string key in uninstallRegistryKey.GetSubKeyNames())
                        {
                            try
                            {


                                using (RegistryKey appRegistryKey = uninstallRegistryKey.OpenSubKey(key, false))
                                {
                                    if (appRegistryKey == null)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        ProgramInfo tempInfo = ToProgramInfo(appRegistryKey);
                                        tempInfo.SetRegistryKey(System.IO.Path.Combine(uninstallRegistryKey.Name, key), RegistryHive.LocalMachine, appRegistryKey.GetValue("DisplayName")?.ToString());
                                        programsinfolist.Add(tempInfo);

                                    }
                                }
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                }
                return programsinfolist;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static List<ProgramInfo> GetAllInstalledPrograms()
        {
            Dictionary<string, ProgramInfo> programsinfolist = new Dictionary<string, ProgramInfo>();
            try
            {
                foreach (ProgramInfo app in GetLocalMachineInstalledPrograms())
                {

                    if (!string.IsNullOrEmpty(app.DisplayName) && !programsinfolist.ContainsKey(app.DisplayName))
                    {
                        programsinfolist[app.DisplayName] = app;
                    }
                }
                foreach (ProgramInfo app in GetCurrentUserInstalledPrograms())
                {
                    if (!string.IsNullOrEmpty(app.DisplayName) && !programsinfolist.ContainsKey(app.DisplayName))
                    {
                        programsinfolist[app.DisplayName] = app;
                    }
                }
                return programsinfolist.Select(ke => ke.Value).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        protected ImageSource GetIcon()
        {
            if (IconPath?.Contains(',') == true)
            {
                string path = "app.exe";
                foreach (string item in IconPath.Split(','))
                {
                    if (System.IO.File.Exists(item))
                    {
                        path = item;
                    }
                }
                IconPath = path;
                return ImageHelper.GetAssociatedIcon(path, true);
            }
            return ImageHelper.GetAssociatedIcon(IconPath, System.IO.File.Exists(IconPath));
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
            return string.IsNullOrEmpty(DisplayName) ? this.RegistryKey : DisplayName;
        }
    }
}