using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Kemorave.SQLite.Attribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SQLitePropertyAttribute : System.Attribute
    {
        public enum DefaultValueBehavior { PopulateAndInclude, Populate, Include }
        public SQLitePropertyAttribute(DefaultValueBehavior defaultValueBehavior = DefaultValueBehavior.PopulateAndInclude, [CallerMemberName] string columnName = null)
        {
            SetName(columnName);
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
        internal static Type SQLiteAttributesType = typeof(SQLitePropertyAttribute);
        internal static Dictionary<string, object> GetIncludeProperties(Type type, PropertyInfo[] props, object obj = null)
        {
            Dictionary<string, object> names = new Dictionary<string, object>();
            foreach (PropertyInfo prop in props)
            {
                foreach (SQLitePropertyAttribute att in prop.GetCustomAttributes(SQLiteAttributesType, true))
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
                foreach (SQLitePropertyAttribute att in prop.GetCustomAttributes(SQLiteAttributesType, true))
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
            object obj;
            foreach (PropertyInfo prop in props)
            {
                foreach (SQLitePropertyAttribute att in prop.GetCustomAttributes(SQLiteAttributesType, true))
                {
                    if (keyValues.ContainsKey(att.Name))
                    {
                        try
                        {
                            obj = keyValues[att.Name];
                            if (obj != System.DBNull.Value)
                            {
                                prop.SetValue(temp, obj);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);
                        }
                    }
                }
            }
        }
    }
}