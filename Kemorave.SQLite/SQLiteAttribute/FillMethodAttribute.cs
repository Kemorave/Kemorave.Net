using System;
using System.Linq;
using System.Reflection;

namespace Kemorave.SQLite.SQLiteAttribute
{
    /// <summary>
    /// A method with <see cref="DataBaseGetter" /> parameter
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]

    
    public class FillMethodAttribute : System.Attribute
    {
        private static readonly Type Type = typeof(FillMethodAttribute);
        public FillMethodAttribute()
        {
        }

        internal static MethodInfo GetFillMethod(Type type)
        {
            return type.GetMethods().FirstOrDefault(s => s.GetCustomAttribute(Type) != null);
        }
    }
}