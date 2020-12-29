using System; 
using System.Linq;
 
namespace  Kemorave.SQLite
{ 
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter)]
    public class SQLiteTableAttribute : Attribute
    {
        public SQLiteTableAttribute(string tableName,string cmd = null)
        {
            // Debug.WriteLine("Attribute "+className);
            TableName = tableName;
            Command = cmd;
        }

        public string TableName { get; }
        public string Command { get; }

        internal static Type SQLiteTableAttributeType = typeof(SQLiteTableAttribute);
        
        public static string GetTableName(Type type)
        {
            return (type.GetCustomAttributes(SQLiteTableAttributeType, true).FirstOrDefault(s => s is SQLiteTableAttribute) as SQLiteTableAttribute)?.TableName;
        }
        internal static TableInfo FromType(Type type)
        {
            TableInfo tableInfo = null;
            foreach (SQLiteTableAttribute item in type.GetCustomAttributes(typeof(SQLiteTableAttribute), true))
            {
                tableInfo = new TableInfo(item.TableName);
                tableInfo.Command = item.Command;
            }
            foreach (var prop in type.GetProperties())
            {
                foreach (SQLiteColumnAttribute item in prop.GetCustomAttributes(typeof(SQLiteColumnAttribute), true))
                {
                    tableInfo.Columns?.Add(item.ColumnInfo);
                }
            }
            return tableInfo;
        }
    } 
}