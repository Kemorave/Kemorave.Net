
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
    public class SQLiteDataBase : IDisposable
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
            TableManager = new TableManager(this);
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
        public SQLiteDataReader ExectuteReader(string cmd, CommandBehavior behavior = CommandBehavior.Default)
        {
            return CreateCommand(cmd).ExecuteReader(behavior);
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
                    //Connection.Dispose();
                    System.IO.File.Delete(path);
                    RefreshConnection(path, dBInfo.Secure?.ToString());
                }
                foreach (TableInfo table in dBInfo.Tables)
                {
                    if (!force)
                    {
                        if (TableManager.TableExist(table.Name))
                        {
                            TableManager.DropTable(table.Name);
                        }
                    }
                    TableManager.CreateTable(table);
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
        public  DataBaseGetter DataGetter { get; }
        public  DataBaseSetter DataSetter { get; }
        public TableManager TableManager { get; }
        public System.Data.SQLite.SQLiteConnection Connection { get; protected set; }
        /// <summary>
        /// Path to database file
        /// </summary>
        public string DataSource => GetDBPath();
    }
}