
using System;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Security;

namespace Kemorave.SQLite
{
    /// <summary>
    /// SQLite databse manager
    /// 
    /// </summary> 
    public class SQLiteDataBase : IDisposable, ISQLiteDataBase
    {
        public SQLiteDataBase(SQLiteConnection connection) : this()
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
        }
        public SQLiteDataBase(string uri, SecureString secure = null) : this()
        {
            RefreshConnection(uri, secure?.ToString());
            if (secure != null)
            {
                Connection.SetPassword(secure.ToString());
            }
            Connection.Open();
        }
        private SQLiteDataBase()
        {
            DataGetter = new DataBaseGetter(this);
            DataSetter = new DataBaseSetter(this);
        }

        #region IDisposable implimentaion
        ~SQLiteDataBase()
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
        #endregion
        public string Backup(string destPath)
        {
            string name = $"SQLite Backup {DateTime.Now}.backup";
            string destfile = System.IO.Path.Combine(destPath, name);
            if (System.IO.File.Exists(destfile))
            {
                System.IO.File.Delete(destfile);
            }

            System.IO.File.Copy(DataSource, destfile);
            return destfile;
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
                string temp = Connection.ConnectionString;
                temp = temp.ToLower().Split(',').FirstOrDefault();
                temp = temp.Replace("data source =", "");
                while (temp[0] == ' ')
                {
                    temp = temp.Remove(0, 1);
                }
                return temp;
            }
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
            return CreateCommand(cmd).ExecuteReader(behavior);
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
            using (SQLiteCommand cmd = CreateCommand(cmdt))
            {
                return cmd.ExecuteNonQuery();
            }
        }
        public SQLiteCommand CreateCommand(string cmd = null)
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
                            this.DropTable(table.Name);
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
        private void RefreshConnection(string filePath, string secure)
        {
            (DataSetter as DataBaseSetter).CheckBusyState();
            Connection?.Dispose();
            Connection = new SQLiteConnection("Data Source = " + filePath);
            if (!string.IsNullOrEmpty(secure))
            {
                Connection.SetPassword(secure);
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
        public IDataBaseGetter DataGetter { get; }
        public IDataBaseSetter DataSetter { get; }
        public System.Data.SQLite.SQLiteConnection Connection { get; protected set; }
        /// <summary>
        ///  When true database commits changes without a chance for rollbacks 
        ///  else use <see cref="SQLiteDataBase.CommitChanges"/> 
        ///  <para/>
        ///  Default is true
        /// </summary>
        public string DataSource => GetDBPath();
    }
}