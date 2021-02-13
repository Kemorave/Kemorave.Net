using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Kemorave.SQLite.Attribute
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]

    public class SQLiteFillMethodAttribute : System.Attribute
    {
        private static readonly Type Type = typeof(SQLiteFillMethodAttribute);
        public SQLiteFillMethodAttribute()
        {
        }

        internal static MethodInfo GetFillMethod(Type type) 
        {
            return  type.GetMethods().FirstOrDefault(s=>s.GetCustomAttribute(Type,true)!=null);
        }
    }
}