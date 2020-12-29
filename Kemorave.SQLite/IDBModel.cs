using System;
using System.Collections.Generic;
using System.Text;

namespace Kemorave.SQLite
{
  public  interface IDBModel
    {
        [Kemorave.SQLite.SQLiteColumn(Kemorave.SQLite.SQLiteColumnAttribute.DefaultValueBehavior.Populate)]
        long ID { get; set; }
    }
}
