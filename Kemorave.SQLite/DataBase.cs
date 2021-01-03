
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
    public class DataBase : IDisposable, IDataBaseGetter
    {
        private bool _isCanceled;
        public System.Data.SQLite.SQLiteConnection Connection { get; protected set; }
        /// <summary>
        ///  When true database commits changes without a chance for rollbacks 
        ///  else use <see cref="DataBase.CommitChanges"/> 
        ///  <para/>
        ///  Default is true
        /// </summary>
        public bool AutoCommitChanges { get; set; } = true;
        public bool IsBusy { get; protected set; }
        public bool CanRollBack => CurrentTransaction != null;
        public string CurrentOperation { get; protected set; }
        protected System.Data.SQLite.SQLiteTransaction CurrentTransaction { get; set; }
        public string DataSource => GetDBPath();

        public const string InsertOperation = "Insert";
        public const string UpdateOperation = "Update";
        public const string DeleteOperation = "Delete";

        #region Other
        public string Backup(string destPath)
        {
            CheckBusyState();
            string name = $"SQLite Backup {DateTime.Now}.backup";
            string destfile = System.IO.Path.Combine(destPath, name);
            if (System.IO.File.Exists(destfile)) System.IO.File.Delete(destfile);
            System.IO.File.Copy(DataSource, destfile);
            return destfile;
        }
        public void CommitChanges()
        {
            CurrentTransaction?.Commit();
            CurrentTransaction = null;
        }
        public void RollBack()
        {
            ExecuteCommand("ROLLBACK");
        }
        public void CancelPendingOperation()
        {
            _isCanceled = true;
        }
        public override string ToString()
        {
            try
            {
                return $"{System.IO.Path.GetFileName(DataSource)} ({DataSource})";
            }
            catch
            {
                return base.ToString();
            }
        }
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
                while (temp[0] == ' ')
                {
                    temp = temp.Remove(0, 1);
                }
                return temp;
            }
        }

        ~DataBase()
        {
            Dispose(true);
        }

        private void Dispose(bool finalizer)
        {
            if (!finalizer)
            {
                GC.SuppressFinalize(this);
            }
            Connection?.Dispose();
        }
        public void Dispose()
        {
            Dispose(false);
        }
        public DataBase(SQLiteConnection connection)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
        }

        public DataBase(string uri, SecureString secure = null)
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
            try
            {
                return this.ExecuteCommand(table.GetCreateCommand());
            }
            finally
            {
                if (!string.IsNullOrEmpty(table.Command))
                {
                    ExecuteCommand(table.Command);
                }
            }
        }
        public int CreateTable(Type type)
        {
            if (SQLiteTableAttribute.FromType(type) is TableInfo tableInfo)
            {
                return CreateTable(tableInfo);
            }
            else
            {
                throw new InvalidOperationException($"Type {type.Name} have no (SQLiteTableAttribute) attribute");
            }
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
        /// <returns><see cref="DataBaseInfo.CommandText"/> affected rows</returns>
        public int Recreate(DataBaseInfo dBInfo, bool force = false)
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
        public int Delete<T>(T item, string tableName = null) where T : class, IDBModel, new()
        {
            if (item == null)
            {
                throw new ArgumentNullException($"{nameof(item)} can't be empty");
            }
            return Delete<T>(new List<T>() { item }, tableName);
        }
        public T GetItemByID<T>(long id, string tableName = null) where T : class, IDBModel, new()
        {
            return GetItems<T>(tableName, "*", "WHERE ID =" + id).FirstOrDefault();
        }
        #endregion


        public int Insert<T>(IList<T> rows, string tableName = null) where T : class, IDBModel
        {
            CheckBusyState();
            try
            {
                OnOperationStart(InsertOperation);

                Type type = typeof(T);
                if (tableName == null)
                {
                    tableName = SQLiteTableAttribute.GetTableName(type);
                }
                CheckParams(tableName, rows);

                int TORE = 0;
                System.Reflection.PropertyInfo[] props = type.GetProperties();

                Dictionary<string, object> keyValues = SQLitePropertyAttribute.GetIncludeProperties(type, props);
                if (keyValues?.Count <= 0)
                {
                    throw new AggregateException($"Type {type.FullName} properties have no SQLite attributes");
                }
                Tuple<string, string> tuple = keyValues.GetInsertNamesAndParameters();

                using (SQLiteCommand command = GetCommand($"INSERT INTO [{tableName}] ({tuple.Item1}) VALUES ({tuple.Item2})"))
                {
                    foreach (T item in rows)
                    {
                        keyValues = SQLitePropertyAttribute.GetIncludeProperties(type, props, item);
                        foreach (KeyValuePair<string, object> val in keyValues)
                        {
                            command.Parameters.Add(new SQLiteParameter(DbType.Object, val.Value));
                        }
                        CheckCancellation();
                        TORE += command.ExecuteNonQuery();
                        command.Parameters.Clear();
                    }
                }
                return TORE;
            }
            finally
            {
                OnOperationEnd();
            }
        }


        public int Update<T>(IList<T> rows, string tableName = null) where T : class, IDBModel
        {
            CheckBusyState();
            try
            {
                OnOperationStart(UpdateOperation);
                Type type = typeof(T);
                if (tableName == null)
                {
                    tableName = SQLiteTableAttribute.GetTableName(type);
                }
                CheckParams<T>(tableName, rows);

                int TORE = 0;


                System.Reflection.PropertyInfo[] props = type.GetProperties();
                Dictionary<string, object> keyValues = SQLitePropertyAttribute.GetIncludeProperties(type, props);
                if (keyValues?.Count <= 0)
                {
                    throw new AggregateException($"Type {type.FullName} properties have no SQLite attributes");
                }
                string parameters = keyValues.GetUpdateParameters();

                using (SQLiteCommand command = GetCommand($"UPDATE {tableName} SET {parameters} WHERE ID = ?"))
                {
                    foreach (T item in rows)
                    {
                        keyValues = SQLitePropertyAttribute.GetIncludeProperties(type, props, item);
                        foreach (KeyValuePair<string, object> val in keyValues)
                        {
                            command.Parameters.Add(new SQLiteParameter(DbType.Object, val.Value));
                        }
                        CheckCancellation();
                        command.Parameters.Add(new SQLiteParameter(DbType.Object, item.ID));
                        TORE += command.ExecuteNonQuery();
                        command.Parameters.Clear();
                    }
                }
                return TORE;
            }
            finally
            {
                OnOperationEnd();
            }
        }
        public int Delete<T>(IList<T> rows, string tableName = null) where T : class, IDBModel
        {
            CheckBusyState();
            try
            {
                OnOperationStart(DeleteOperation);
                Type type = typeof(T);

                if (tableName == null)
                {
                    tableName = SQLiteTableAttribute.GetTableName(type);
                }
                CheckParams(tableName, rows);

                int TORE = 0;

                using (SQLiteCommand command = GetCommand($"DELETE FROM  {tableName} WHERE ID = ?"))
                {
                    foreach (T item in rows)
                    {
                        command.Parameters.Add(new SQLiteParameter(DbType.Object, item.ID));

                        CheckCancellation();
                        TORE += command.ExecuteNonQuery();
                        command.Parameters.Clear();
                    }
                }
                return TORE;
            }
            finally
            {
                OnOperationEnd();
            }
        }
        public int DropTable(string tableName)
        {
            return ExecuteCommand(TableInfo.GetDropCommand(tableName));
        }
        public int ClearTable(string tableName)
        {
            return ExecuteCommand(TableInfo.GetClearCommand(tableName));
        }
        public IEnumerable<T> GetItems<T>(string tableName = null, string selection = "*", string condition = null) where T : class, IDBModel, new()
        {
            Type type = typeof(T);
            if (tableName == null)
            {
                tableName = SQLiteTableAttribute.GetTableName(type);
            }
            System.Reflection.PropertyInfo[] props = type.GetProperties();
            using (SQLiteDataReader reader = GetCommand($"SELECT {selection} FROM {tableName} {condition}").ExecuteReader())
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


        #region Operaion
        private void CheckParams<T>(string tableName, IList<T> rows) where T : class, IDBModel
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException(nameof(tableName));
            }
            if (rows == null)
            {
                throw new ArgumentNullException($"{nameof(rows)} can't be empty or null");
            }
            if (rows.Count == 0)
            {
                throw new InvalidOperationException("Items list has no items");
            }
        }

        private void CheckCancellation()
        {
            if (_isCanceled)
            {
                throw new OperationCanceledException("Operation " + CurrentOperation + " is cancelled");
            }
        }

        protected virtual void OnOperationEnd()
        {
            IsBusy = false;
            _isCanceled = false;
            CurrentOperation = null;
            if (AutoCommitChanges)
            {
                CommitChanges();
            }
        }

        private void CheckBusyState()
        {
            if (IsBusy)
            {
                throw new DatabaseBusyException("Database is busy with" + CurrentOperation + " operation");
            }
            else
            {
                CommitChanges();
            }
        }

        protected virtual void OnOperationStart(string operation)
        {
            IsBusy = true;
            CurrentOperation = operation;
            CurrentTransaction = Connection.BeginTransaction(System.Data.IsolationLevel.Serializable);
        }

        #endregion
        #endregion


    }
}