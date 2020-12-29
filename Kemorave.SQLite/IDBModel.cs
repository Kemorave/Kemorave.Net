using System;
using System.Collections.Generic;
using System.Text;

namespace Kemorave.SQLite
{
  public  interface IDBModel
    {
        [Kemorave.SQLite.SQLiteProperty(Kemorave.SQLite.SQLitePropertyAttribute.DefaultValueBehavior.Populate)]
        long ID { get; set; }
    }
}
