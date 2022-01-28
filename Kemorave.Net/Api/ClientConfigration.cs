using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace Kemorave.Net.Api
{
    public class ClientConfigration : IDisposable
    {
        public ClientConfigration(Uri baseAddress,IEnumerable<KeyValuePair<string, IEnumerable<string>>> defaultRequestHeaders = null)
        {

            HttpClient = new System.Net.Http.HttpClient
            {
                BaseAddress = baseAddress
            };

            if (defaultRequestHeaders?.Count() > 0)
            {
                foreach (System.Collections.Generic.KeyValuePair<string, System.Collections.Generic.IEnumerable<string>> item in defaultRequestHeaders)
                {
                    HttpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }
        }

        public System.Net.Http.HttpClient HttpClient { get; }
        public void Dispose()
        {
            HttpClient?.Dispose();
        }
    }
}