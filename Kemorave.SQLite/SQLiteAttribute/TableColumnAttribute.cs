using System;
using System.Runtime.CompilerServices;

namespace Kemorave.SQLite.SQLiteAttribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TableColumnAttribute : Attribute, IColumnAttribute
    {
        
        public TableColumnAttribute([CallerMemberName]
         string name = null,
                 SQLiteType sQLiteType = SQLiteType.TEXT,
                 bool isPrimaryKey = false,
                 bool isAutoIncrement = false,
                 bool isNull = true,
                 bool isUnique = false,
                 string defaultValue = null,
                 string extra = null)
        {
            ColumnInfo = new ColumnInfo(name, sQLiteType, isPrimaryKey, isAutoIncrement)
            { IsUNIQUE = isUnique, DefaultValue = defaultValue, Extra = extra, IsNullable = isNull };
        }
        
        public ColumnInfo ColumnInfo { get; private set; }
    }
}