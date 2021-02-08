using System.Data;
using System.Data.SQLite;

namespace Kemorave.SQLite
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISQLiteDataBase
    {
        SQLiteDataReader ExectuteReader(string cmd, CommandBehavior behavior = CommandBehavior.Default);
        SQLiteCommand CreateCommand(string cmd = null);
        int ExecuteCommand(string cmdt);
        SQLiteConnection Connection { get; }
        string DataSource { get; }
    }
}