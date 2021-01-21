using System;
using System.Collections.Generic;
using System.Text;

namespace Kemorave.Net
{
    public static class NetUtil
    {
        public static bool CheckInternetActivity()
        {

            try
            {
                using (var client = new System.Net.WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                    return true;
            }
            catch
            {
                return false;
            }
        }
    }
}