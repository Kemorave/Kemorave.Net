using System;
using System.Collections.Generic;
using System.Linq;

namespace Kemorave.SQLite.SQLiteAttribute
{
    
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]

    public class ForeignKeyAttribute : Attribute, IColumnAttribute
    {
        public ForeignKeyAttribute(string foreignKey,
        SQLiteType sQLiteType = SQLiteType.INTEGER,
         bool isNull = false,
         bool isUnique = false,
         string defaultValue = null,
         string parentTableName = null,
         string parentTableRefID = null,
        SQLiteActions onDeleteAction = SQLiteActions.NO_ACTION,
       SQLiteActions onUpdateAction = SQLiteActions.NO_ACTION)
        {
            ColumnInfo = new ColumnInfo(foreignKey, sQLiteType, false, false, parentTableName, parentTableRefID)
            { IsUNIQUE = isUnique, OnDeleteAction = onDeleteAction, OnUpdateAction = onUpdateAction, DefaultValue = defaultValue,  IsNullable = isNull };
        }
        internal static IEnumerable<ColumnInfo> GetTableForeignKeys(Type tableType)
        {
            foreach (var prop in tableType.GetProperties())
            {
                foreach (ForeignKeyAttribute att in prop.GetCustomAttributes(typeof(ForeignKeyAttribute), true))
                {
                    yield return att?.ColumnInfo;
                }
            }
        }
        internal static ColumnInfo GetTableForeignKey(Type includeTableType, string mainTableName)
        {
            return GetTableForeignKeys(includeTableType).FirstOrDefault(a => a.ParentTable == mainTableName);
        }
        public ColumnInfo ColumnInfo { get; }
    }
}