namespace Kemorave.Net
{
    public static class NetUtil
    {
        public static bool IsConnectedToInternet()
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
    }
}