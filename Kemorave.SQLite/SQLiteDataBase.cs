
using System;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using Kemorave.SQLite.ModelBase;
using Kemorave.SQLite.Options;

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
        public SQLiteDataBase(string uri, string secure = null) : this()
        {
            RefreshConnection(uri, secure); 
        }
        private SQLiteDataBase()
        {
            Getter = new DataGetter(this);
            Setter = new DataSetter(this);
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
        public string Backup(string destPath, string name = null, bool overwrite = false)
        {
            if (name == null)
            {
                name = $"Database Backup {DateTime.Now.ToFileTimeUtc()}.sqlite";
            }

            string destfile = System.IO.Path.Combine(destPath, name);

            System.IO.File.Copy(DataSource, destfile, overwrite);
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
        public SQLiteCommand CreateCommand<T>(SelectOptions<T> options) where T : IDBModel, new()
        {
            SQLiteCommand cmd = new SQLiteCommand(options.GetCommand(), Connection);
            if (options.Where != null)
            {
                foreach (WhereConditon[] con in options.Where.Conditons)
                {
                    foreach (WhereConditon item in con)
                    {
                        if (item.HasParameters)
                        {
                            cmd.Parameters.Add(new SQLiteParameter(dbType: System.Data.DbType.Object, value: item.Value));
                        }
                    }
                }
            }
            return cmd;
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
            Setter.CheckBusyState();
            Connection?.Dispose();
            Connection = new SQLiteConnection("Data Source = " + filePath);
            if (!string.IsNullOrEmpty(secure))
            {
                Connection.SetPassword(secure);
            }
            Connection.Open();
        } 
        public DataGetter Getter { get; }
        public DataSetter Setter { get; }
        public TableManager TableManager { get; }
        public SQLiteConnection Connection { get; protected set; }
        /// <summary>
        /// Path to database file
        /// </summary>
        public string DataSource => GetDBPath();
    }
}