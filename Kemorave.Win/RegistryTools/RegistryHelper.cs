using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Win32;

namespace Kemorave.Win.RegistryTools
{
    public class RegistryHelper
    {
        public static readonly bool Is64BitOS = Environment.Is64BitOperatingSystem;
        public static void LaunchCurrentApplicationOnStartup(bool isEnabled)
        {
            const string StartupReg = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
            using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(StartupReg, true))
            {
                Assembly ca = Assembly.GetExecutingAssembly();

                if (isEnabled)
                {
                    key.SetValue(ca.GetName().Name, ca.Location);
                }
                else
                {
                    key.DeleteSubKey(System.IO.Path.Combine(StartupReg, ca.GetName().Name));
                }
            }
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
        public static IEnumerable<ProgramInfo> GetInstalledPrograms(RegistryKey uninstallRegistryKey)
        {
            if (uninstallRegistryKey == null)
            {
                throw new ArgumentNullException(nameof(uninstallRegistryKey));
            }
            string name;
            foreach (string key in uninstallRegistryKey.GetSubKeyNames())
            {
                ProgramInfo tempInfo = null;
                try
                {
                    using (RegistryKey appRegistryKey = uninstallRegistryKey.OpenSubKey(key, false))
                    {
                        name = appRegistryKey?.GetValue("DisplayName")?.ToString();
                        if (!string.IsNullOrEmpty(name))
                        {
                            tempInfo = ProgramInfo.ToProgramInfo(appRegistryKey);
                            tempInfo.SetRegistryKey(System.IO.Path.Combine(uninstallRegistryKey.Name, key), RegistryHive.LocalMachine, name);
                        }
                    }
                }
                catch
                {
                    continue;
                }
                if (tempInfo != null)
                {
                    yield return (tempInfo);
                }
            }
        }
        public static IEnumerable<ProgramInfo> GetLocalMachineInstalledPrograms()
        {
            using (RegistryKey uninstallRegistryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", RegistryKeyPermissionCheck.Default, System.Security.AccessControl.RegistryRights.ReadKey))
            {
                foreach (ProgramInfo item in (GetInstalledPrograms(uninstallRegistryKey)))
                {
                    yield return item;
                }
            }
            if (Is64BitOS)
            {
                using (RegistryKey uninstallRegistryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall"))
                {
                    foreach (ProgramInfo item in (GetInstalledPrograms(uninstallRegistryKey)))
                    {
                        yield return item;
                    }
                }
            }
        }
        public static IEnumerable<ProgramInfo> GetCurrentUserInstalledPrograms()
        {
            using (RegistryKey uninstallRegistryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", RegistryKeyPermissionCheck.ReadSubTree, System.Security.AccessControl.RegistryRights.ReadKey))
            {
                foreach (ProgramInfo item in (GetInstalledPrograms(uninstallRegistryKey)))
                {
                    yield return item;
                }
            }
        }
        public static IEnumerable<ProgramInfo> GetAllInstalledPrograms()
        {
            Dictionary<string, ProgramInfo> programsinfolist = new Dictionary<string, ProgramInfo>();
            try
            {
                foreach (ProgramInfo app in GetLocalMachineInstalledPrograms())
                {

                    if (!programsinfolist.ContainsKey(app.DisplayName))
                    {
                        programsinfolist[app.DisplayName] = app;
                    }
                }
                foreach (ProgramInfo app in GetCurrentUserInstalledPrograms())
                {
                    if (!programsinfolist.ContainsKey(app.DisplayName))
                    {
                        programsinfolist[app.DisplayName] = app;
                    }
                }
                return programsinfolist.Values;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}