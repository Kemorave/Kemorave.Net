using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Kemorave.Net
{
   public static class AuthUtil
    {
        public static AuthenticationHeaderValue Basic(string name, string passwrd)
        {

            return  AuthenticationHeaderValue.Parse( "Basic " + System.Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes($"{name}:{passwrd}")));

        }
        public static AuthenticationHeaderValue Bearer(string token)
        {
            return AuthenticationHeaderValue.Parse("Bearer " +token);

        }
    }
}
