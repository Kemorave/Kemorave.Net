
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Security;

namespace Kemorave.SQLite
{
    public enum SQLiteType
    {
        INT,
        INTEGER,
        TINYINT,
        SMALLINT,
        MEDIUMINT,
        BIGINT,
        UNSIGNED_BIG_INT,
        INT2,
        INT8,
        NUMERIC,
        DECIMAL,
        BOOLEAN,
        DATE,
        DATETIME,
        REAL,
        DOUBLE,
        DOUBLE_PRECISION,
        FLOAT,
        CHARACTER,
        VARCHAR,
        VARYING_CHARACTER,
        NCHAR,
        NATIVE_CHARACTER,
        NVARCHAR,
        TEXT,
        CLOB,
        BLOB
    }
    /// <summary>
    /// SQLite databse manager
    /// 
    /// </summary> 
    public class SQLiteDb : IDisposable
    {

        public System.Data.SQLite.SQLiteConnection Connection { get; protected set; }
        public string DataSource => GetDBPath();
        #region Other

        private string GetDBPath()
        {
            if (string.IsNullOrEmpty(Connection?.ConnectionString))
            {
                return null;
            }
            else
            {
                string temp = null;
                temp = Connection.ConnectionString;
                temp = temp.ToLower().Split(',').FirstOrDefault();
                temp = temp.Replace("data source =", "");
                return temp;
            }
            return null;
        }

        ~SQLiteDb()
        {
            Dispose(true);
        }

        private void Dispose(bool fin)
        {
            if (!fin)
            {
                GC.SuppressFinalize(this);
            }
            Connection?.Dispose();
        }
        public void Dispose()
        {
            Dispose(false);
        }
        public SQLiteDb(SQLiteConnection connection)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
        }

        public SQLiteDb(string uri, SecureString secure = null)
        {
            RefreshConnection(uri, secure?.ToString());
            if (secure != null)
            {
                Connection.SetPassword(secure.ToString());
            }
            Connection.Open();
        }


        public bool TableExist(string table_name)
        {
            using (SQLiteDataReader reader = ExectuteReader($"SELECT 1 FROM sqlite_master WHERE type='table' AND name='{table_name}';", CommandBehavior.SingleRow))
            {
                return reader.HasRows;
            }
        }
        public bool TempTableExist(string table_name)
        {
            using (SQLiteDataReader reader = ExectuteReader($"SELECT 1 FROM sqlite_temp_master WHERE type='table' AND name='{table_name}';", CommandBehavior.SingleRow))
            {
                return reader.HasRows;
            }
        }

        public SQLiteDataReader ExectuteReader(string cmd, CommandBehavior behavior = CommandBehavior.Default)
        {
            return GetCommand(cmd).ExecuteReader(behavior);
        }

        public DataTable GetDataTable(string cmd)
        {
            using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(cmd, Connection))
            {
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                return dataTable;
            }
        }

        public int CreateTable(TableInfo table)
        {
            return this.ExecuteCommand(table.GetCreateCommand());
        }
        public int ExecuteCommand(string cmdt)
        {
            using (SQLiteCommand cmd = GetCommand(cmdt))
            {
                return cmd.ExecuteNonQuery();
            }
        }
        public SQLiteCommand GetCommand(string cmd = null)
        {
            if (cmd != null)
            {
                return new SQLiteCommand(cmd, Connection);
            }

            return new SQLiteCommand(Connection);
        }
        /// <summary>
        /// Recreates database
        /// </summary>
        /// <param name="force">When true the database file is deleted whole</param>
        /// <param name="dBInfo">Database info</param>
        /// <returns><see cref="DBInfo.CommandText"/> affected rows</returns>
        public int Recreate(DBInfo dBInfo, bool force = false)
        {
            if (System.IO.File.Exists(DataSource))
            {
                if (force)
                {
                    string path = DataSource;
                    Connection.Shutdown();
                    Connection.Dispose();
                    System.IO.File.Delete(path);
                    RefreshConnection(path, dBInfo.Secure?.ToString());
                }
                foreach (TableInfo table in dBInfo.Tables)
                {
                    if (!force)
                    {
                        if (TableExist(table.Name))
                        {
                            DropTable(table.Name);
                        }
                    }
                    CreateTable(table);
                }
                if (!string.IsNullOrEmpty(dBInfo.CommandText))
                {
                    return ExecuteCommand(dBInfo.CommandText);
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                throw new System.IO.FileNotFoundException("Source file not found");
            }
        }

        private void RefreshConnection(string dataSource, string secure)
        {
            Connection?.Dispose();
            Connection = new SQLiteConnection("Data Source = " + dataSource);
            if (!string.IsNullOrEmpty(secure))
            {
                Connection.SetPassword(secure);
            }
        }

        #endregion

        #region CRUDE
        #region Single
        public int Insert<T>(T item, string tableName = null) where T : class, IDBModel
        {
            if (item == null)
            {
                throw new ArgumentNullException($"{nameof(item)} can't be empty");
            }
            return Insert<T>(new List<T>() { item }, tableName);
        }
        public int Update<T>(T item, string tableName = null) where T : class, IDBModel, new()
        {
            if (item == null)
            {
                throw new ArgumentNullException($"{nameof(item)} can't be empty");
            }
            return Update<T>(new List<T>() { item }, tableName);
        }
        public int Delete<T>( T item, string tableName=null) where T : class, IDBModel, new()
        {
            if (item == null)
            {
                throw new ArgumentNullException($"{nameof(item)} can't be empty");
            }
            return Delete<T>( new List<T>() { item },tableName);
        }
        public T GetItemByID<T>( long id, string tableName=null) where T : class, IDBModel, new()
        {
            return GetItems<T>(tableName, "*", "WHERE ID =" + id).FirstOrDefault();
        }
        #endregion
        public int Insert<T>( IList<T> rows, string tableName=null) where T : class, IDBModel
        {
            Type type = typeof(T);
            if (tableName==null)
            {
                tableName= SQLiteTableAttribute.GetTableName(type);
            }
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException(nameof(tableName));
            }
            if (rows == null)
            {
                throw new ArgumentNullException($"{nameof(rows)} can't be empty");
            }
            if (rows.Count == 0)
            {
                throw new InvalidOperationException("Nothing to insert");
            }
            int TORE = 0;

            System.Reflection.PropertyInfo[] props = type.GetProperties();
            Dictionary<string, object> keyValues = SQLiteColumnAttribute.GetIncludeProperties(type, props);
            if (keyValues?.Count <= 0)
            {
                throw new AggregateException($"Type {type.FullName} properties have no SQLite attributes");
            }
            Tuple<string, string> tuple = keyValues.GetInsertNamesAndParameters();

            using (SQLiteTransaction trans = Connection.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                using (SQLiteCommand command = GetCommand($"INSERT INTO [{tableName}] ({tuple.Item1}) VALUES ({tuple.Item2})"))
                {
                    foreach (T item in rows)
                    {
                        keyValues = SQLiteColumnAttribute.GetIncludeProperties(type, props, item);
                        foreach (KeyValuePair<string, object> val in keyValues)
                        {
                            command.Parameters.Add(new SQLiteParameter(DbType.Object, val.Value));
                        }
                        TORE += command.ExecuteNonQuery();
                        command.Parameters.Clear();
                    }
                }
                trans.Commit();
            }
            return TORE;
        }
        public int Update<T>(IList<T> rows, string tableName = null) where T : class, IDBModel
        {
            Type type = typeof(T);
            if (tableName == null)
            {
                tableName = SQLiteTableAttribute.GetTableName(type);
            }
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException(nameof(tableName));
            }
            if (rows == null)
            {
                throw new ArgumentNullException($"{nameof(rows)} can't be empty");
            }
            if (rows.Count == 0)
            {
                throw new InvalidOperationException("Nothing to insert");
            }
            int TORE = 0;

          
            System.Reflection.PropertyInfo[] props = type.GetProperties();
            Dictionary<string, object> keyValues = SQLiteColumnAttribute.GetIncludeProperties(type, props);
            if (keyValues?.Count <= 0)
            {
                throw new AggregateException($"Type {type.FullName} properties have no SQLite attributes");
            }
            string parameters = keyValues.GetUpdateParameters();

            using (SQLiteTransaction trans = Connection.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                using (SQLiteCommand command = GetCommand($"UPDATE {tableName} SET {parameters} WHERE ID = ?"))
                {
                    foreach (T item in rows)
                    {
                        keyValues = SQLiteColumnAttribute.GetIncludeProperties(type, props, item);
                        foreach (KeyValuePair<string, object> val in keyValues)
                        {
                            command.Parameters.Add(new SQLiteParameter(DbType.Object, val.Value));
                        }
                        command.Parameters.Add(new SQLiteParameter(DbType.Object, item.ID));
                        TORE += command.ExecuteNonQuery();
                        command.Parameters.Clear();
                    }
                }
                trans.Commit();
            }
            return TORE;
        }
        public int Delete<T>(IList<T> rows, string tableName = null) where T : class, IDBModel
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException(nameof(tableName));
            }
            if (rows == null)
            {
                throw new ArgumentNullException($"{nameof(rows)} can't be empty");
            }
            if (rows.Count == 0)
            {
                throw new InvalidOperationException("Nothing to insert");
            }
            int TORE = 0;
            using (SQLiteTransaction trans = Connection.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                using (SQLiteCommand command = GetCommand($"DELETE FROM  {tableName} WHERE ID = ?"))
                {
                    foreach (T item in rows)
                    {
                        command.Parameters.Add(new SQLiteParameter(DbType.Object, item.ID));

                        TORE += command.ExecuteNonQuery();
                        command.Parameters.Clear();
                    }
                }
                trans.Commit();
            }
            return TORE;
        }
        public int DropTable(string tableName)
        {
            return ExecuteCommand(TableInfo.GetDropCommand(tableName));
        }
        public int ClearTable(string tableName)
        {
            return ExecuteCommand(TableInfo.GetClearCommand(tableName));
        }
        public IEnumerable<T> GetItems<T>(string tableName=null, string selection = "*", string condition = null) where T : class, IDBModel, new()
        {
            Type type = typeof(T);
            if (tableName == null)
            {
                tableName = SQLiteTableAttribute.GetTableName(type);
            }
            System.Reflection.PropertyInfo[] props = type.GetProperties();
            using (SQLiteDataReader reader = GetCommand($"SELECT {selection} FROM {tableName} {condition}").ExecuteReader())
            {
                Dictionary<string, object> values = SQLiteColumnAttribute.GetPopulateProperties(type, props);
                if (values.Count == 0)
                {
                    throw new AggregateException($"Type {type.FullName} properties have no SQLite attributes");
                }
                T temp = new T();
                Dictionary<string, object> keyValues = new Dictionary<string, object>(values);
                while (reader.Read())
                {
                    foreach (KeyValuePair<string, object> item in values)
                    {
                        keyValues[item.Key] = reader[item.Key];
                    }
                    SQLiteColumnAttribute.SetProperties(in temp, props, keyValues);
                    yield return temp;
                }
            }
        }
        #endregion


    }
}