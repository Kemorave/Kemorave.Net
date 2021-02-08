using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace Kemorave.SQLite
{
    internal class DataBaseGetter : IDataBaseGetter
    {
        private readonly ISQLiteDataBase DataBase;
        public DataBaseGetter(ISQLiteDataBase dataBase)
        {
            DataBase = dataBase ?? throw new ArgumentNullException(nameof(dataBase));
        }

        public T GetItemByID<T>(long id, string tableName = null) where T : class, IDBModel, new()
        {
            return GetItems<T>(tableName, "*", "WHERE ID =" + id).FirstOrDefault();
        }
        public System.Data.DataTable GetDataTable(string cmd)
        {
            using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(cmd, DataBase.Connection))
            {
                System.Data.DataTable dataTable = new System.Data.DataTable();
                dataAdapter.Fill(dataTable);
                return dataTable;
            }
        }
        public IEnumerable<T> GetItems<T>(string tableName = null, string selection = "*", string condition = null) where T : class, IDBModel, new()
        {
            Type type = typeof(T);
            if (tableName == null)
            {
                tableName = SQLiteTableAttribute.GetTableName(type);
            }
            System.Reflection.PropertyInfo[] props = type.GetProperties();
            using (SQLiteDataReader reader = DataBase.CreateCommand($"SELECT {selection} FROM {tableName} {condition}").ExecuteReader())
            {
                Dictionary<string, object> values = SQLitePropertyAttribute.GetPopulateProperties(type, props);
                if (values.Count == 0)
                {
                    throw new AggregateException($"Type {type.FullName} properties have no SQLite attributes");
                }
                T temp = new T();
                System.Reflection.MethodInfo fillMethod = SQLiteFillMethodAttribute.GetFillMethod(type);
                object[] fillMethodInvArgs = null;
                if (fillMethod != null)
                {
                    fillMethodInvArgs = new object[] { this };
                }
                Dictionary<string, object> keyValues = new Dictionary<string, object>(values);
                while (reader.Read())
                {
                    foreach (KeyValuePair<string, object> item in values)
                    {
                        keyValues[item.Key] = reader[item.Key];
                    }
                    SQLitePropertyAttribute.SetProperties(in temp, props, keyValues);
                    fillMethod?.Invoke(temp, fillMethodInvArgs);
                    yield return temp;
                }
            }
        }
    }
}