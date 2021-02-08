
using Microsoft.Win32;

namespace Kemorave.Win.RegistryTools
{
 public class RegistryItemBase 
 {
        public virtual string DisplayName { get; protected set; } = string.Empty;
  public virtual RegistryHive RegistryHive { get; protected set; }
  public virtual string RegistryKey { get; protected set; }
  public virtual void SetRegistryKey(string RegistryKey, RegistryHive RegistryHive, string KeyDisplayName)
  {
   this.RegistryKey = RegistryKey;
   this.RegistryHive = RegistryHive;
   this.DisplayName = KeyDisplayName??string.Empty;
  }
 }
}