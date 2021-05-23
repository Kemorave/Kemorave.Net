
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Kemorave.SQLite
{
    public static class Tools
    {
        #region Refletion/Tools

        internal static object GetPropValue(object obj, Type type, string name)
        {
            if (obj == null)
            {
                return null;
            }
            foreach (string part in name.Split('.'))
            {
                if (obj == null) { return null; }
                PropertyInfo info = type.GetProperty(part);
                if (info == null) { return null; }

                obj = info.GetValue(obj, null);
            }
            return obj;
        }
        internal static Tuple<string, string> GetInsertNamesAndParameters(this Dictionary<string, object> keyValues)
        {
            string parames = string.Empty, names = string.Empty;
            foreach (KeyValuePair<string, object> item in keyValues)
            {
                if (names == string.Empty)
                {
                    names += item.Key;
                    parames += "?";
                }
                else
                {
                    names += $",{item.Key}";
                    parames += ",?";
                }
            }
            return new Tuple<string, string>(names, parames);
        }
        internal static string GetUpdateParameters(this Dictionary<string, object> keyValues)
        {
            string parames = string.Empty;
            foreach (KeyValuePair<string, object> item in keyValues)
            {
                if (parames == string.Empty)
                {
                    parames += item.Key + "=?";
                }
                else
                {
                    parames += $",{item.Key}=?";
                }
            }
            return parames;
        }

        #endregion
         
        #region Security

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
                throw new ArgumentException(  nameof(value));
            }

            return Convert.ToBase64String(SHA1.Create().ComputeHash(Convert.FromBase64String(value)) );
        }
        #endregion
    }
}