
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Kemorave.SQLite
{
    public static class Tools
    {
        #region Refletion/Tools

        internal static Object GetPropValue(Object obj, Type type, String name)
        {
            if (obj == null)
            {
                return null;
            }
            foreach (String part in name.Split('.'))
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

        public static string GetColumnsCreationInfo(this IEnumerable<ColumnInfo> list)
        {
            String info = string.Empty;
            int count = 0;
            foreach (var column in list)
            {
                if (count == 0)
                {
                    info += column.GetCreationInfo();
                }
                else
                {
                    info += "," + column.GetCreationInfo(); ;
                }
                count++;
            } 
            foreach (var column in list.Where(c => c.IsForeignKey))
            {
               
                    info += "," + column.GetCreationInfo();
                 
            }
            return info;
        }
        #region Security

        #endregion
        public static string EncryptString(string encryptString, String EncryptionKey)
        {
            if (String.IsNullOrEmpty(encryptString))
            {
                return encryptString;
            }
            if (String.IsNullOrEmpty(EncryptionKey))
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
        public static string DecryptString(String cipherText, String EncryptionKey)
        {
            if (String.IsNullOrEmpty(cipherText))
            {
                return cipherText;
            }
            if (String.IsNullOrEmpty(EncryptionKey))
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
    }
}