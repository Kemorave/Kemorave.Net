using System;
using System.Runtime.CompilerServices;

namespace Kemorave.SQLite
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SQLiteColumnAttribute : Attribute
    {

        public SQLiteColumnAttribute([CallerMemberName]
         string name = null,
         SQLiteType sQLiteType = SQLiteType.TEXT,
         bool isPrimaryKey = false,
         bool isAutoIncrement = false,
         bool isNull = false,
         bool isUnique = false,
         string defaultValue = null,
         string parentTableName = null,
         string parentTableRefID = null,
         ColumnInfo.SQLiteActions onDeleteAction = ColumnInfo.SQLiteActions.NO_ACTION,
         ColumnInfo.SQLiteActions onUpdateAction = ColumnInfo.SQLiteActions.NO_ACTION,
         string extra = null)
        {
            ColumnInfo = new ColumnInfo(name, sQLiteType, isPrimaryKey, isAutoIncrement, parentTableName, parentTableRefID)
            { IsUNIQUE = isUnique, OnDeleteAction = onDeleteAction, OnUpdateAction = onUpdateAction, DefaultValue = defaultValue, Extra = extra, IsNullable = isNull };
        }
        public SQLiteColumnAttribute([CallerMemberName]
         string name = null,
                 SQLiteType sQLiteType = SQLiteType.TEXT,
                 bool isPrimaryKey = false,
                 bool isAutoIncrement = false,
                 bool isNull = false,
                 bool isUnique = false,
                 string defaultValue = null,
                 string extra = null)
        {
            ColumnInfo = new ColumnInfo(name, sQLiteType, isPrimaryKey, isAutoIncrement)
            { IsUNIQUE = isUnique, DefaultValue = defaultValue, Extra = extra, IsNullable = isNull };
        }
        public SQLiteColumnAttribute([CallerMemberName]
         string name = null,
               SQLiteType sQLiteType = SQLiteType.TEXT,
               bool isPrimaryKey = false,
               bool isAutoIncrement = false,
               bool isNull = false)
        {
            ColumnInfo = new ColumnInfo(name, sQLiteType, isPrimaryKey, isAutoIncrement)
            { IsNullable = isNull };
        }

        public ColumnInfo ColumnInfo { get; private set; }
    }
}