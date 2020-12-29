using System;

namespace Kemorave.SQLite
{
    public class TableInfo
    {
        public TableInfo(string name)
        {
            Name = name;
            Columns = new System.Collections.ObjectModel.Collection<ColumnInfo>();
        }
        public string Name { get; }
        public System.Collections.ObjectModel.Collection<ColumnInfo> Columns { get; }
        public string Command { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="NullReferenceException"/>
        /// <exception cref="InvalidOperationException"/>
        /// <returns></returns>
        public string GetCreateCommand()
        {
            if (Columns == null)
            {
                throw new NullReferenceException($"{nameof(Columns)} can't be null");
            }
            if (Columns.Count == 0)
            {
                throw new InvalidOperationException($"{nameof(Columns)} have no columns");
            }
            return $"CREATE TABLE IF NOT EXISTS {Name} ({Columns?.GetColumnsCreationInfo()})";
        }
        public static string GetDropCommand(string name)
        {
            return $"DROP TABLE {name}";
        }
        public static string GetClearCommand(string name)
        {
            return $"DELETE FROM {name}";
        }
        public string GetDropCommand()
        {
            return GetDropCommand(Name);
        }

        public string GetClearCommand()
        {
            return GetClearCommand(Name);
        }

        public override string ToString()
        {
            return $"{Name} ({Columns?.GetColumnsCreationInfo()})";
        }
    }
}