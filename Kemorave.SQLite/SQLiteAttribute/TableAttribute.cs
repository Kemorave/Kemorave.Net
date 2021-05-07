using System;
using System.Collections.Generic;
using System.Linq;

namespace Kemorave.SQLite.SQLiteAttribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter)]
    public class TableAttribute : System.Attribute
    {
        public TableAttribute(string tableName, string cmd = null)
        {
            // Debug.WriteLine("Attribute "+className);
            TableName = tableName;
            Command = cmd;
        }

        public string TableName { get; }
        public string Command { get; }

        internal static Type SQLiteTableAttributeType = typeof(TableAttribute);
        public static Dictionary<string, string> Keys = new Dictionary<string, string>();
        public static string GetTableName(Type type)
        {
            if (Keys.ContainsKey(type.FullName))
            {
                return Keys[type.FullName];
            }
            else
            {
                Keys[type.FullName] = (type.GetCustomAttributes(SQLiteTableAttributeType, true).FirstOrDefault(s => s is TableAttribute) as TableAttribute)?.TableName ?? type.Name; ;
            }
            return Keys[type.FullName];
        }
        internal static TableInfo FromType(Type type)
        {
            TableInfo tableInfo = null;
            if (type.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() is TableAttribute item)
            {
                tableInfo = new TableInfo(item.TableName)
                {
                    Command = item.Command
                };
                foreach (var prop in type.GetProperties())
                {
                    foreach (TableColumnAttribute coll in prop.GetCustomAttributes(typeof(TableColumnAttribute), true))
                    {
                        tableInfo.Columns?.Add(coll.ColumnInfo);
                    }
                }
            }
            return tableInfo;
        }
    }
}