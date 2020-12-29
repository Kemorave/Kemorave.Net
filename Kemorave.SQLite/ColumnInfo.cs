using System;

namespace Kemorave.SQLite
{
    public class ColumnInfo
    {
        public enum SQLiteActions
        {/// <summary>
         /// Nothing will happen, this is default sction
         /// </summary>
            NO_ACTION,
            /// <summary>
            /// Value will be sett to <see cref="null"/>
            /// </summary>
            SET_NULL,
            /// <summary>
            /// Value will be set to <see cref="ColumnInfo.DefaultValue"/> 
            /// </summary>
            SET_DEFAULT,
            /// <summary>
            /// Does not allow you to change or delete values in the parent key of the parent table.
            /// </summary>
            RESTRICT,
            /// <summary>
            /// Propagates the changes from the parent table to the child table when you update or delete the parent key.
            /// </summary>
            CASCADE
        }
        private bool _IsNullable;
        /// <summary>
        /// Creates a column in database
        /// </summary>
        /// <param name="columnName">name</param>
        /// <param name="type">data type</param>
        public ColumnInfo(string columnName, SQLiteType type)
        {
            ColumnName = columnName ?? throw new ArgumentNullException(nameof(columnName));
            Type = type;
        }

        /// <summary>
        /// Creates a column in database
        /// </summary>
        /// <param name="columnName">name</param>
        /// <param name="type">data type</param>
        /// <param name="isPrimaryKey">Set column as the primary key for table</param>
        /// <param name="isAutoIncrement">Each row in column will have diffrent incremental value</param>
        public ColumnInfo(string columnName, SQLiteType type, bool isPrimaryKey,bool isAutoIncrement=true) : this(columnName, type)
        {
            IsPrimaryKey = isPrimaryKey;
            IsAutoIncrement = isAutoIncrement;
        }
        /// <summary>
        /// Creates a column as a foreign key 
        /// </summary>
        /// <param name="columnName">name</param>
        /// <param name="type">data type</param>
        /// <param name="parentTable">Refrenced table</param>
        /// <param name="parentTableRefID">Refrenced table column name (usualy the same as column name)</param>
        public ColumnInfo(string columnName, SQLiteType type, string parentTable, string parentTableRefID = null) : this(columnName, type)
        {
            ParentTableRefID = parentTableRefID ?? columnName;
            ParentTable = parentTable ?? throw new ArgumentNullException(nameof(parentTable));
            IsForeignKey = true;
        }
        public ColumnInfo(ColumnInfo columnInfo, string parentTable, string parentTableRefID = null) : this(columnInfo.ColumnName, columnInfo.Type)
        {
            ParentTableRefID = parentTableRefID ?? columnInfo.ColumnName;
            ParentTable = parentTable ?? throw new ArgumentNullException(nameof(parentTable));
            IsForeignKey = true;
        }

        public string ColumnName { get; }
        public SQLiteType Type { get; }
        public bool IsPrimaryKey { get; }
        public string DefaultValue { get; set; }
        public bool IsUNIQUE { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        public bool IsNullable
        {
            get => _IsNullable;
            set
            {
                if (IsPrimaryKey && value)
                {
                    throw new InvalidOperationException($"Null values not allowed while {nameof(IsPrimaryKey)} is set to True");
                }
                else
                {
                    _IsNullable = value;
                }
            }
        }
        public bool IsAutoIncrement { get; set; }
        public bool IsForeignKey { get; }

        public string ParentTable { get; }
        public string ParentTableRefID { get; }
        public string Extra { get; set; }
        public SQLiteActions OnUpdateAction { get; set; }
        public SQLiteActions OnDeleteAction { get; set; }
        public string GetForeignKeyCreationInfo()
        {
            ColumnInfo tableColumn = this;
            if (!IsForeignKey)
            {
                return null;
            }
            string Command = $" FOREIGN KEY ({ColumnName}) REFERENCES {ParentTable}({ParentTableRefID}) ";
            return Command;
        }
        public string GetCreationInfo()
        {
            ColumnInfo tableColumn = this;
            string Command = tableColumn.ColumnName + " " + tableColumn.Type.ToString().Replace("_", " ");

            if (tableColumn.IsPrimaryKey)
            {
                Command += " PRIMARY KEY ";
            }
            if (tableColumn.IsNullable && !tableColumn.IsPrimaryKey)
            {
                Command += " NULL ";
            }
            if (tableColumn.IsAutoIncrement)
            {
                Command += " AUTOINCREMENT ";
            }
           
            if (!string.IsNullOrEmpty(DefaultValue))
            {
                Command += $" DEFAULT '{DefaultValue}' ";
            }
            if (tableColumn.IsUNIQUE)
            {
                Command += " UNIQUE ";
            }
            if (!string.IsNullOrEmpty(Extra))
            {
                Command += Extra;
            }

            return Command;
        }
        public override string ToString()
        {
            return GetCreationInfo();
        }
    }
}