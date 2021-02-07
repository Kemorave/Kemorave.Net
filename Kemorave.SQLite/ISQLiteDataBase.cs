using System.Data;
using System.Data.SQLite;

namespace Kemorave.SQLite
{
    public interface ISQLiteDataBase
    {
        SQLiteConnection Connection { get; }
        string DataSource { get; }
        string Backup(string destPath);
        SQLiteDataReader ExectuteReader(string cmd, CommandBehavior behavior = CommandBehavior.Default);
        int ExecuteCommand(string cmdt);
        SQLiteCommand GetCommand(string cmd = null);
    }
}