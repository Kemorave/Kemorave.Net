using System;
using System.Runtime.CompilerServices;

namespace Kemorave.SQLite.Attribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SQLiteTableColumnAttribute : System.Attribute
    {
        /// <summary>
        /// Creates a foreignKey column 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sQLiteType"></param>
        /// <param name="isPrimaryKey"></param>
        /// <param name="isAutoIncrement"></param>
        /// <param name="isNull"></param>
        /// <param name="isUnique"></param>
        /// <param name="defaultValue"></param>
        /// <param name="parentTableName"></param>
        /// <param name="parentTableRefID"></param>
        /// <param name="onDeleteAction"></param>
        /// <param name="onUpdateAction"></param>
        /// <param name="extra"></param>
        public SQLiteTableColumnAttribute([CallerMemberName]
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
        public SQLiteTableColumnAttribute([CallerMemberName]
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
        public SQLiteTableColumnAttribute([CallerMemberName]
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