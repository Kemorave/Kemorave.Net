﻿using System;

namespace Kemorave.SQLite
{
    public partial class ColumnInfo
    {
        private bool _IsNullable;
        private bool _IsUNIQUE;

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
        public ColumnInfo(string columnName, SQLiteType type, bool isPrimaryKey, bool isAutoIncrement = true) : this(columnName, type)
        {
            IsPrimaryKey = isPrimaryKey;
            IsAutoIncrement = isAutoIncrement;
        }
        /// <summary>
        /// Creates a column in database
        /// </summary>
        /// <param name="columnName">name</param>
        /// <param name="type">data type</param>
        /// <param name="isPrimaryKey">Set column as the primary key for table</param>
        /// <param name="isAutoIncrement">Each row in column will have diffrent incremental value</param>
        public ColumnInfo(string columnName, SQLiteType type, bool isPrimaryKey, bool isAutoIncrement ,string parentTable, string parentTableRefID = null) : this(columnName, type,parentTable,parentTableRefID)
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
        }
        public ColumnInfo(ColumnInfo columnInfo, string parentTable, string parentTableRefID = null) : this(columnInfo.ColumnName, columnInfo.Type)
        {
            ParentTableRefID = parentTableRefID ?? columnInfo.ColumnName;
            ParentTable = parentTable ?? throw new ArgumentNullException(nameof(parentTable));
         }

        public string ColumnName { get; }
        public SQLiteType Type { get; }
        public bool IsPrimaryKey { get; }
        public string DefaultValue { get; set; }
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
        public bool IsUNIQUE
        {
            get => _IsUNIQUE;
            set
            {
                if (IsPrimaryKey && value)
                {
                    throw new InvalidOperationException($"Uique not allowed while {nameof(IsPrimaryKey)} is set to True");
                }
                else
                {
                    _IsUNIQUE = value;
                }
            }
        }
        public bool IsAutoIncrement { get; set; }
        public bool IsForeignKey { get => !string.IsNullOrEmpty(ParentTable); }

        public string ParentTable { get; }
        public string ParentTableRefID { get; }
        public string Extra { get; set; }
        public SQLiteActions OnUpdateAction { get; set; }
        public SQLiteActions OnDeleteAction { get; set; }
        public string GetForeignKeyCreationInfo()
        {
          //  ColumnInfo tableColumn = this;
            if (!IsForeignKey)
            {
                return null;
            }
            string Command = $" FOREIGN KEY ({ColumnName}) REFERENCES {ParentTable}({ParentTableRefID}) ";
            if (OnDeleteAction != SQLiteActions.NO_ACTION)
            {
                Command += $" ON DELETE {OnDeleteAction.ToString().Replace("_", string.Empty)}";
            }
            if (OnUpdateAction != SQLiteActions.NO_ACTION)
            {
                Command += $" ON UPDATE {OnUpdateAction.ToString().Replace("_", string.Empty)}";
            }
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
           if (!string.IsNullOrEmpty(DefaultValue))
            {
                Command += $" DEFAULT \'{DefaultValue}\' ";
            }
            if (tableColumn.IsAutoIncrement)
            {
                Command += " AUTOINCREMENT ";
            }
            
            if (tableColumn.IsNullable && !tableColumn.IsPrimaryKey)
            {
                Command += " NULL ";
            }
            else
            {
                Command += " NOT NULL ";
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