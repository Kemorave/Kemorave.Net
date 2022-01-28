using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kemorave.Net
{
    public static class NetUtil
    {
        public static bool IsConnected()
        {

            try
            {
                using (System.Net.WebClient client = new System.Net.WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        
        public static StringContent GetJsonContent(object content)
        {
            return new StringContent(Serialize(content), Encoding.UTF8, "application/json");
        }
        public static T Deserialize<T>(string text)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(text);
        }

        public static string Serialize(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

        public static void DownloadFileAsync(string url, string filePath, Action<double> feedBack, CancellationToken cancellationToken)
        {
            System.Net.WebClient client = new System.Net.WebClient();
            cancellationToken.Register(() => { client.CancelAsync(); });
            try
            {
                client.DownloadProgressChanged += (s, a) =>
                {
                    feedBack(a.ProgressPercentage);
                };
                client.DownloadFileAsync(new Uri(url), filePath);
            }
            catch (OperationCanceledException e)
            {
                System.IO.File.Delete(filePath);
            }
            catch (System.Net.WebException e)
            {

            }
            catch (HttpRequestException e)
            {

            }
            catch (Exception e)
            {
                throw;
            }
        }

        public static async Task<string> GetContent(this HttpResponseMessage res)
        {
            string content = await res.Content.ReadAsStringAsync();

            try
            {
                res.EnsureSuccessStatusCode();
                return content;
            }
            catch (Exception e)
            {
                if (!string.IsNullOrEmpty(content))
                {
                    ResponceMessage message = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponceMessage>(content);
                    message.Code = (int)res.StatusCode;
                    throw new ResponceExeption(message);
                }
                else
                {
                    throw new ResponceExeption(new ResponceMessage() { Code = (int)res.StatusCode, Message = e.Message });
                }

            }

        }

        #region Security
        public static string GetMD5Hash(string v)
        {
            if (string.IsNullOrWhiteSpace(v))
            {
                throw new ArgumentException("message", nameof(v));
            }

            return System.Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(v)));
        }
        public static string GetSHA1Hash(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(nameof(value));
            }

            return Convert.ToBase64String(SHA1.Create().ComputeHash(Convert.FromBase64String(value)));
        }
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

        #endregion
    }
}