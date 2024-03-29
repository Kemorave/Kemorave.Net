﻿
using System;
using System.Security;
using Kemorave.SQLite.SQLiteAttribute;

namespace Kemorave.SQLite
{
    public class DataBaseInfo
    {
        public DataBaseInfo(string path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Tables = new System.Collections.ObjectModel.Collection<TableInfo>();
        }

        public DataBaseInfo(string path, SecureString secure) : this(path)
        {
            Secure = secure ?? throw new ArgumentNullException(nameof(secure));
        }

        public DataBaseInfo(string path, string command) : this(path)
        {
            CommandText = command ?? throw new ArgumentNullException(nameof(command));
        }

        public DataBaseInfo(string path, SecureString secure, string command) : this(path, secure)
        {
            CommandText = command ?? throw new ArgumentNullException(nameof(command));
        }
        public void AddTableFromType(Type type)
        {
            if (TableAttribute.FromType(type) is TableInfo tableInfo)
            {
                Tables.Add(tableInfo);
            }
            else
            {
                throw new InvalidOperationException($"Type {type.Name} have no (SQLiteTableAttribute) attribute");
            }
        }
        public string Path { get; }
        public SecureString Secure { get; }
        /// <summary>
        /// A command executed after creating all tables
        /// </summary>
        public string CommandText { get; }
        public System.Collections.ObjectModel.Collection<TableInfo> Tables { get; }
        public string GetCreationCommand()
        {
            string cmd = string.Empty;
            if (Tables?.Count > 0)
            {
                foreach (var table in Tables)
                {
                    cmd += $" {table.GetCreateCommand()} ";
                }
            }
            return cmd;
        }
    }
}