using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Kemorave.SQLite.SQLiteAttribute;

namespace Kemorave.SQLite
{
    /// <summary>
    /// Provide means to access database tables
    /// </summary>
    public class TableManager
    {
        private readonly SQLiteDataBase dataBase;

        internal TableManager(SQLiteDataBase dataBase)
        {
            this.dataBase = dataBase ?? throw new ArgumentNullException(nameof(dataBase));
        }
        public bool TableExist(string table_name)
        {
            using (SQLiteDataReader reader = dataBase.ExectuteReader($"SELECT 1 FROM sqlite_master WHERE type='table' AND name='{table_name}';", CommandBehavior.SingleRow))
            {
                return reader.HasRows;
            }
        }
        public bool TempTableExist(string table_name)
        {
            using (SQLiteDataReader reader = dataBase.ExectuteReader($"SELECT 1 FROM sqlite_temp_master WHERE type='table' AND name='{table_name}';", CommandBehavior.SingleRow))
            {
                return reader.HasRows;
            }
        }
        public int CreateTable(TableInfo table)
        {
            try
            {
                return dataBase.ExecuteCommand(table.GetCreateCommand());
            }
            finally
            {
                if (!string.IsNullOrEmpty(table.Command))
                {
                    dataBase.ExecuteCommand(table.Command);
                }
            }
        }
        public int DropTable(string tableName)
        {
            return dataBase.ExecuteCommand(TableInfo.GetDropCommand(tableName));
        }
        public int ClearTable(string tableName)
        {
            return dataBase.ExecuteCommand(TableInfo.GetClearCommand(tableName));
        }
        public int GetTableCount(string tableName)
        {
            return dataBase.ExecuteCommand($" SELECT count(*) FROM {tableName}; ");
        }

        public int CreateTable(Type type)
        {
            if (TableAttribute.FromType(type) is TableInfo tableInfo)
            {
                return CreateTable(tableInfo);
            }
            else
            {
                throw new InvalidOperationException($"Type {type.Name} have no (SQLiteTableAttribute) attribute");
            }
        }
        public string[] GetTablesNames()
        {
            List<string> names = new List<string>();
            using (SQLiteDataReader reader = dataBase.ExectuteReader($"SELECT name FROM sqlite_master WHERE type='table';"))
            {
                while (reader.Read())
                {
                    names.Add(reader["name"].ToString());
                }
            }
            return names.ToArray();
        }
        public TableInfo GetTableInfo(string name)
        {
            TableInfo tableInfo = new TableInfo(name);
            ColumnInfo column;
            using (SQLiteDataReader reader = dataBase.CreateCommand($"PRAGMA table_info('{name}')").ExecuteReader())
            {
                while (reader.Read())
                {
                    int columnId = reader.GetInt32(reader.GetOrdinal("cid"));
                    string columnName = reader.GetString(reader.GetOrdinal("name"));
                    string type = reader.GetString(reader.GetOrdinal("type"));
                    if (type.IndexOf("(") is int indexOfP)
                    {
                        if (indexOfP > 0)
                        {
                            type = type.Substring(0, indexOfP);
                        }
                    }
                    bool notNull = reader.GetBoolean(reader.GetOrdinal("notnull"));
                    object defaultValue = reader.GetValue(reader.GetOrdinal("dflt_value"));
                    bool primaryKey = reader.GetBoolean(reader.GetOrdinal("pk"));
                    if (primaryKey)
                    {
                        column = new ColumnInfo(columnName, (SQLiteType)Enum.Parse(typeof(SQLiteType), type), true);
                    }
                    else
                    {
                        column = new ColumnInfo(columnName, (SQLiteType)Enum.Parse(typeof(SQLiteType), type))
                        {
                            IsNullable = !notNull
                        };
                    }
                    column.DefaultValue = defaultValue?.ToString();
                    tableInfo.Columns.Add(column);
                }
            }
            return tableInfo;
        }

    }
}