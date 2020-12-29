using System;
using System.Linq;
using System.Security;

namespace Kemorave.Net.Api
{
    public class ApiConfigration : IDisposable
    {
        public ApiConfigration(Uri uri, string key, SecureString secure) : this(uri, key)
        {
            if (secure == null)
            {
                throw new ArgumentNullException(nameof(secure));
            }
            HttpClient.DefaultRequestHeaders.Add("Secure", secure?.ToString());
        }
        public ApiConfigration(Uri uri, string key, SecureString[] secure) : this(uri, key)
        {
            if (secure==null)
            {
                throw new ArgumentNullException(nameof(secure));
            }
            HttpClient.DefaultRequestHeaders.Add("Secure", secure.Select(a=>a.ToString()));
        }
        public ApiConfigration(Uri uri, string key) : this(uri)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            HttpClient.DefaultRequestHeaders.Add("Key", key);
        }

        public ApiConfigration(Uri uri)
        {
            Uri = uri ?? throw new ArgumentNullException(nameof(uri));
            HttpClient = new System.Net.Http.HttpClient
            {
                BaseAddress = Uri
            };
        }
       
        public System.Net.Http.HttpClient HttpClient { get; }
        public Uri Uri { get; }
        protected string Key { get; }
        public void Dispose()
        {
            HttpClient?.Dispose();
        }
    }
}