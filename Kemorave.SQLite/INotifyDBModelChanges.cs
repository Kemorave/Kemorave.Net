using System;
using System.Collections.Generic;
using System.Text;

namespace Kemorave.SQLite
{
   public interface INotifyDBModelChanges
    {
        event EventHandler<KeyValuePair<string, object>> PropertyDataChanged;
    }
}
