using System.Reflection;

namespace Kemorave.Win.RegistryTools
{
 public class RegistryHelper
 {
  public static void LaunchCurrentApplicationOnStartup(bool isEnabled)
  {
   Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
   Assembly ca = Assembly.GetExecutingAssembly();
   if (isEnabled)
   {
    key.SetValue(ca.GetName().Name, ca.Location);
   }
   else
   {
    key.DeleteSubKey(ca.GetName().Name);
   }
   key.Close();
  }
  //public static List<ProgramInfo> GetAllInstalledPrograms()
  //{
  // List<ProgramInfo> programsinfolist = new List<ProgramInfo>();
  // try
  // {

  // }
  // catch (Exception)
  // {
  //  return null;
  // }
  //}


 }
}