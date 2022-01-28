using Kemorave.SQLite.ModelBase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Kemorave.SQLite.SQLiteAttribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public partial class PropertyAttribute : Attribute
    {
        public PropertyAttribute(SqlPropertyHandling defaultValueBehavior = SqlPropertyHandling.PopulateAndInclude, [CallerMemberName] string columnName = null)
        {
            SetName(columnName);
            SetDefaultValueBehavior(defaultValueBehavior);
        }

        public string Name { get; private set; }
        public SqlPropertyHandling DefaultBehavior { get; private set; }
        public void SetDefaultValueBehavior(SqlPropertyHandling defaultValueBehavior)
        {
            DefaultBehavior = defaultValueBehavior;
        }
        public void SetName(string name)
        {
            Name = name;
        }
        internal static Type SQLiteAttributesType = typeof(PropertyAttribute);
        internal static Dictionary<string, object> GetIncludeProperties(Type type, PropertyInfo[] props, object obj = null)
        {
            Dictionary<string, object> names = new Dictionary<string, object>();
            foreach (PropertyInfo prop in props)
            {
                object[] aat = prop.GetCustomAttributes(SQLiteAttributesType, true);
                if (aat?.Length == 0 || aat == null)
                {
                    names.Add(prop.Name, Tools.GetPropValue(obj, type, prop.Name));
                    continue;
                }
                foreach (PropertyAttribute att in prop.GetCustomAttributes(SQLiteAttributesType, true))
                {
                    switch (att?.DefaultBehavior)
                    {
                        case SqlPropertyHandling.Ignore:
                            break;
                        case SqlPropertyHandling.Populate:
                            break;
                        default:
                            names.Add(att.Name, Tools.GetPropValue(obj, type, prop.Name));
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
                object[] aat = prop.GetCustomAttributes(SQLiteAttributesType, true);
                if (aat?.Length == 0 || aat == null)
                {
                    names.Add(prop.Name, Tools.GetPropValue(obj, type, prop.Name));
                    continue;
                }
                foreach (PropertyAttribute att in aat)
                {
                    switch (att?.DefaultBehavior)
                    {
                        case SqlPropertyHandling.Ignore:
                            break;
                        case SqlPropertyHandling.Include:
                            break;
                        default:
                            names.Add(att.Name, Tools.GetPropValue(obj, type, prop.Name));
                            break;
                    }

                }

            }
            return names;
        }

        private static readonly Type typeOfBool = typeof(bool);

        internal static void SetProperties<T>(in T temp, PropertyInfo[] props, Dictionary<string, object> keyValues) where T : IDBModel, new()
        {
            object obj;
            foreach (PropertyInfo prop in props)
            {

                if (keyValues.ContainsKey(prop.Name))
                {
                    try
                    {
                        obj = keyValues[prop.Name];
                        if (obj == DBNull.Value)
                        {
                            prop.SetValue(temp, null);
                        }
                        else if (prop.PropertyType.Equals(typeOfBool))
                        {
                            prop.SetValue(temp, obj?.ToString()?.Equals("1") == true || (bool.TryParse(obj?.ToString(),out bool v)&&v) ? true : false);
                        }
                        else
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