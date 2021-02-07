using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kemorave.Win.Reflection
{
 public static class ReflectionHelper
 {
  #region Local var
  private static Func<object, object> _LastGetter;
  private static Tuple<Type, string> _LastProprty = new Tuple<Type, string>(typeof(object), "");
  private static readonly Dictionary<Tuple<Type, string>, Func<object, object>> _funcList = new Dictionary<Tuple<Type, string>, Func<object, object>>();
  #endregion
  private static Func<object, object> CreateGetter(Type type, string PropertyName)
  {
   try
   {
    if (_LastProprty.Item1 == type && _LastProprty.Item2 == PropertyName)
    {
     return _LastGetter;
    }
    else
    {
     var _Lastentry = _funcList.FirstOrDefault(key => (key.Key.Item1 == type && key.Key.Item2 == PropertyName));
     if (_Lastentry.Value != null)
     {
      _LastGetter = _Lastentry.Value;
      _LastProprty = _Lastentry.Key;
      return _LastGetter;
     }
     _LastProprty = new Tuple<Type, string>(type, PropertyName);
    }
    var PropertyInfo = type.GetProperty(PropertyName);
    var obj = System.Linq.Expressions.Expression.Parameter(typeof(object), "obj");
    var objT = System.Linq.Expressions.Expression.TypeAs(obj, type);
    var property = System.Linq.Expressions.Expression.Property(objT, PropertyInfo);
    var convert = System.Linq.Expressions.Expression.TypeAs(property, typeof(object));
    _LastGetter = (Func<object, object>)System.Linq.Expressions.Expression.Lambda(convert, obj).Compile();
    _funcList.Add(new Tuple<Type, string>(type, PropertyName), _LastGetter);
    return _LastGetter;
   }
   catch (Exception)
   {

   }
   return null;
  }

  public static object GetItemPropertyValue(object item, string PropertyName, object[] index = null)
  {
   try
   {
    if (PropertyName.Contains('|'))
    {
     foreach (var prop in PropertyName.Split('|'))
     {
      if (GetItemPropertyValue(item, prop, index) is object value)
      {
       return value;
      }
     }
    }
    if (PropertyName.Contains("."))
    {
     object value = null;
     var propertiesChain = PropertyName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
     for (int i = 0; i < propertiesChain.Length; i++)
     {
      if (i == 0)
       value = GetItemPropertyValue(item, propertiesChain[i], index);
      else
      {
       var temp = GetItemPropertyValue(value, propertiesChain[i], index);
       value = new object();
       value = temp;
      }
     }
     return value;
    }
    else
    {
     return CreateGetter(item.GetType(), PropertyName)(item);
    }
   }
   catch (Exception e)
   {
    Debug.WriteLine(e.ToString());
    return null;
   }
  }

 }
}