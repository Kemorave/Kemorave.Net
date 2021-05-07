using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Kemorave.SQLite.SQLiteAttribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public partial class PropertyAttribute : System.Attribute
    {
        public PropertyAttribute(Behavior defaultValueBehavior = Behavior.PopulateAndInclude, [CallerMemberName] string columnName = null)
        {
            SetName(columnName);
            SetDefaultValueBehavior(defaultValueBehavior);
        }

        public string Name { get; private set; }
        public Behavior DefaultBehavior { get; private set; }
        public void SetDefaultValueBehavior(Behavior defaultValueBehavior)
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
                }
                else
                {
                    foreach (PropertyAttribute att in prop.GetCustomAttributes(SQLiteAttributesType, true))
                    {
                        switch (att.DefaultBehavior)
                        {
                            case Behavior.Include:
                                names.Add(att.Name, Tools.GetPropValue(obj, type, prop.Name));
                                break;
                            case Behavior.PopulateAndInclude:
                                names.Add(att.Name, Tools.GetPropValue(obj, type, prop.Name));
                                break;
                            default:
                                break;
                        }

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
                    names.Add(prop.Name, null);
                }
                else
                {
                    foreach (PropertyAttribute att in aat)
                    {
                        if (att != null)
                        {
                            switch (att.DefaultBehavior)
                            {
                                case Behavior.Populate:
                                    names.Add(att.Name, Tools.GetPropValue(obj, type, prop.Name));
                                    break;
                                case Behavior.PopulateAndInclude:
                                    names.Add(att.Name, Tools.GetPropValue(obj, type, prop.Name));
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            return names;
        }
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
                        if (obj != DBNull.Value)
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