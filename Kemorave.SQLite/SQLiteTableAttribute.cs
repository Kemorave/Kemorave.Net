﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Kemorave.SQLite
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter)]
    public class SQLiteTableAttribute : Attribute
    {
        public SQLiteTableAttribute([CallerMemberName] string className = null)
        {
           // Debug.WriteLine("Attribute "+className);
            TableName = className;
        }
        public   string TableName { get;  }
        internal static Type SQLiteTableAttributeType = typeof(SQLiteTableAttribute);

        public static string GetTableName(Type type)
        {
            return (type.GetCustomAttributes(SQLiteTableAttributeType,true).FirstOrDefault(s => s is SQLiteTableAttribute) as SQLiteTableAttribute)?.TableName;
        }
    }
}
