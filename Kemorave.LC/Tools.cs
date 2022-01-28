using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
namespace Kemorave_LC_Generator
{
	internal class Tools
	{
		public static string EncryptString(string encryptString, string EncryptionKey)
		{
			if (string.IsNullOrEmpty(encryptString))
			{
				return encryptString;
			}
			if (string.IsNullOrEmpty(EncryptionKey))
			{
				return encryptString;
			}
			encryptString = encryptString.Replace('-', '+').Replace('_', '/');
			byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
			using (Aes encryptor = Aes.Create())
			{
				Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
				encryptor.Key = pdb.GetBytes(32);
				encryptor.IV = pdb.GetBytes(16);
				using (MemoryStream ms = new MemoryStream())
				{
					using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
					{
						cs.Write(clearBytes, 0, clearBytes.Length);

					}
					encryptString = Convert.ToBase64String(ms.ToArray());
				}
			}
			return encryptString;
		}
		public static string DecryptString(string cipherText, string EncryptionKey)
		{
			if (string.IsNullOrEmpty(cipherText))
			{
				return cipherText;
			}
			if (string.IsNullOrEmpty(EncryptionKey))
			{
				return cipherText;
			}
			cipherText = cipherText.Replace(" ", "+");

			cipherText = cipherText.Replace('-', '+').Replace('_', '/');
			byte[] cipherBytes = Convert.FromBase64String(cipherText);
			using (Aes encryptor = Aes.Create())
			{
				Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
				encryptor.Key = pdb.GetBytes(32);
				encryptor.IV = pdb.GetBytes(16);
				using (MemoryStream ms = new MemoryStream())
				{
					using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
					{
						cs.Write(cipherBytes, 0, cipherBytes.Length);
					}
					cipherText = Encoding.Unicode.GetString(ms.ToArray());
				}
			}
			return cipherText;
		}
		public static string Hash(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return value;
			}
			value  = value.Replace('-', '+').Replace('_', '/'); 
			using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
			{
				byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(value);
				byte[] hashBytes = md5.ComputeHash(inputBytes);

				// Convert the byte array to hexadecimal string
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < hashBytes.Length; i++)
				{
					sb.Append(hashBytes[i].ToString("X2"));
				}
				return sb.ToString();
			}
		}
	}
}
