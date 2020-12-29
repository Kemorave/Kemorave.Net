using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Kemorave.SQLite
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class SQLiteColumnAttribute : Attribute
    {
        public enum DefaultValueBehavior { PopulateAndInclude, Ignore, Populate, Include }
        public SQLiteColumnAttribute(DefaultValueBehavior defaultValueBehavior = DefaultValueBehavior.PopulateAndInclude, [CallerMemberName] string propertyName = null)
        {
            SetName(propertyName);
            SetDefaultValueBehavior(defaultValueBehavior);
        }
        public string Name { get; private set; }
        public DefaultValueBehavior DefaultBehavior { get; private set; }
        public void SetDefaultValueBehavior(DefaultValueBehavior defaultValueBehavior)
        {
            DefaultBehavior = defaultValueBehavior;
        }
        public void SetName(string name)
        {
            Name = name;
        }
        internal static Type SQLiteAttributesType = typeof(SQLiteColumnAttribute);


        internal static Dictionary<string, object> GetIncludeProperties(Type type,PropertyInfo[] props, object obj = null)
        {
            Dictionary<string, object> names = new Dictionary<string, object>();
            foreach (PropertyInfo prop in props)
            {
                foreach (SQLiteColumnAttribute att in prop.GetCustomAttributes(SQLiteAttributesType, true))
                {
                    switch (att.DefaultBehavior)
                    {
                        case DefaultValueBehavior.Include:
                            names.Add(att.Name, Tools.GetPropValue(obj, type, prop.Name));
                            break;
                        case DefaultValueBehavior.PopulateAndInclude:
                            names.Add(att.Name, Tools.GetPropValue(obj, type, prop.Name));
                            break;
                        default:
                            break;
                    }

                }
            }
            return names;
        }
        internal static Dictionary<string, object> GetPopulateProperties(Type type, PropertyInfo[] props, object obj = null)
        {
            Dictionary<string, object> names = new Dictionary<string, object>();
            foreach (PropertyInfo prop in props)
            {
                foreach (SQLiteColumnAttribute att in prop.GetCustomAttributes(SQLiteAttributesType, true))
                {
                    switch (att.DefaultBehavior)
                    {
                        case DefaultValueBehavior.Populate:
                            names.Add(att.Name, Tools.GetPropValue(obj, type, prop.Name));
                            break;
                        case DefaultValueBehavior.PopulateAndInclude:
                            names.Add(att.Name, Tools.GetPropValue(obj, type, prop.Name));
                            break;
                        default:
                            break;
                    }

                }
            }
            return names;
        }

        internal static void SetProperties<T>(in T temp, PropertyInfo[] props, Dictionary<string, object> keyValues) where T : class, IDBModel, new()
        {
            foreach (PropertyInfo prop in props)
            {
                foreach (SQLiteColumnAttribute att in prop.GetCustomAttributes(SQLiteAttributesType, true))
                {
                    if (keyValues.ContainsKey(att.Name))
                    {
                        prop.SetValue(temp, keyValues[att.Name]); 
                    } 
                }
            }
        }
    }
}