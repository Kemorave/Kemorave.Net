using System;
using System.IO;
using System.Security;

using Kemorave_LC_Generator;

namespace Kemorave.LC
{
	public class Manager
	{
		private static string Key { get; set; }
		private static string UserName { get; set; }
		public static void SetKey(string k, string u)
		{
			Key = k;
			UserName = u;
		}
		public static bool ValidateCode(string vak)
		{
			return GetCode() == (vak);
		}
		public static void ValidateCode(IVal vak, string text)
		{
			SecureString s = new SecureString();
			foreach (char item in GetShortenCode(text))
			{
				s.AppendChar(item);
			}
			s.MakeReadOnly();
			vak.Secure = s;
		}
		public static bool ValidateKeyFile(string filePath)
		{
			return File.Exists(filePath) ? Tools.Hash(GetCode()) == File.ReadAllText(filePath) : false;
		}
		public static void SaveKeyFile(string filePath)
		{
			File.WriteAllText(filePath, Tools.Hash(GetCode()));
		}

		private static string GetCodedName()
		{
			string userN = null;
			if (Environment.MachineName?.Length > 0)
			{
				int us = Environment.MachineName.Length / 2;
				userN = Environment.MachineName.Substring(us, Environment.MachineName.Length - us);
			}
			if (Environment.OSVersion?.ToString()?.Length > 0)
			{
				int us = Environment.OSVersion.ToString().Length / 2;
				userN += Environment.OSVersion.ToString().Substring(us, Environment.OSVersion.ToString().Length - us);
			}
			string ha = Tools.Hash(userN + Environment.MachineName).ToLower();
			ha = ha.Substring(0, ha.Length < 16 ? ha.Length : 16);
			return ha;
		}
		private static string GetCode()
		{
			return GetShortenCode(GetCodedName());
		}
		private static string GetShortenCode(string pcpiece)
		{
			string code = Tools.EncryptString(pcpiece, Key).ToLower();
			if (code.Length > 16)
			{
				code = code.Substring(0, 16);
			}
			return code;
		}

		public static Statment GetStatment()
		{
			return new Statment("Licensing required", "This application is licensed to only " + UserName, $"Please send the following code (Or screenshot this message):\n\n\"{GetCodedName()}\"\n\nTo the system developer to get the activation code or by calling +249116720907 or email kemorave@gmail.com");
		}
		public interface IVal
		{
			SecureString Secure { get; set; }
		}
		public class Statment
		{
			internal Statment(string title, string midTitle, string lowTitle)
			{
				Title = title ?? throw new ArgumentNullException(nameof(title));
				MidTitle = midTitle ?? throw new ArgumentNullException(nameof(midTitle));
				LowTitle = lowTitle ?? throw new ArgumentNullException(nameof(lowTitle));
			}

			public string Title { get; set; }
			public string MidTitle { get; set; }
			public string LowTitle { get; set; }
		}
	}
}
