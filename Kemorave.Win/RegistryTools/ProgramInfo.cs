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
		ProgramInfo()
		{
		}
		private static ProgramInfo ToProgramInfo(RegistryKey appRegistryKey)
		{
			DateTime InstallDate = DateTime.MinValue;
			var pr = new ProgramInfo()
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
				EstimatedSize = Int64.TryParse(appRegistryKey.GetValue("EstimatedSize")?.ToString(), out long EstimatedSize) ? EstimatedSize * 1024 : 0,
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
			if (!string.IsNullOrEmpty(uninstallString))
			{
				ProcessStartInfo procStartInfo = new ProcessStartInfo();
				if (uninstallString.ToLower().Contains("msiexec"))
				{
					uninstallString = uninstallString.Replace("\"", "");
					uninstallString = GetCommandInCommaAndArgumentsOutsideByExe(uninstallString);
				}
				else
				{
					if (uninstallString.StartsWith(@""""))
					{
						int indexOfLastComma = uninstallString.IndexOf("\"", 1) + 1;
						procStartInfo.FileName = uninstallString.Substring(0, indexOfLastComma);
						procStartInfo.Arguments = uninstallString.Substring(indexOfLastComma, uninstallString.Length - indexOfLastComma);

					}
					else
					{
						procStartInfo.FileName = "cmd.exe";
						procStartInfo.Arguments = "/c " + GetCommandInCommaAndArgumentsOutsideByExe(uninstallString);
					}
				}
				Process.Start(procStartInfo).WaitForExit();
			}
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
            foreach (String keyName in key.GetSubKeyNames())
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
            using(key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"))
			foreach (String keyName in key.GetSubKeyNames())
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
			using(key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall"))
			foreach (String keyName in key.GetSubKeyNames())
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
						foreach (var key in uninstallRegistryKey.GetSubKeyNames())
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
						foreach (var key in uninstallRegistryKey.GetSubKeyNames())
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
						foreach (var key in uninstallRegistryKey.GetSubKeyNames())
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
			List<ProgramInfo> programsinfolist = new List<ProgramInfo>();
			try
			{
				foreach (var app in GetLocalMachineInstalledPrograms())
				{
					if (app == null)
						continue;
					if (programsinfolist.FirstOrDefault(ap => app.DisplayName == ap.DisplayName) != null)
					{
						continue;
					}
					else
					{
						programsinfolist.Add(app);
					}
				}
				return programsinfolist;
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
				foreach (var item in IconPath.Split(','))
				{
					if (System.IO.File.Exists(item))
					{
						path = item;
					}
				}
				IconPath = path;
				return ImageHelper.GetAssociatedIcon(path,   true);
			}
			return ImageHelper.GetAssociatedIcon(IconPath,   System.IO.File.Exists(IconPath));
		}

		public virtual ImageSource AssociatedIcon
		{
			get => GetIcon();
		}
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
			return DisplayName;
		}
	}
}