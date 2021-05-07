using Kemorave.SQLite.Options;
using Kemorave.SQLite.SQLiteAttribute;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;

namespace Kemorave.SQLite
{
    public class DataBaseGetter  
    {

        private readonly SQLiteDataBase _dataBase;
        public DataBaseGetter(SQLiteDataBase dataBase)
        {
            _dataBase = dataBase ?? throw new ArgumentNullException(nameof(dataBase));
        }
       
        public System.Data.DataTable GetDataTable(string cmd)
        {
            using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(cmd, _dataBase.Connection))
            {
                System.Data.DataTable dataTable = new System.Data.DataTable();
                dataAdapter.Fill(dataTable);
                return dataTable;
            }
        }

        public IEnumerable<IncludeModel> IncludeThrough<Model, IncludeModel, ThroughModel>(Model item, IncludeOptions<Model, IncludeModel> includeOptions = null, Type type = null) where Model : ModelBase.Model, IDBModel, new() where IncludeModel :  IDBModel,  new() where ThroughModel :  IDBModel, new()
        {
            if (includeOptions == null)
            {
                includeOptions = new IncludeOptions<Model, IncludeModel>();

            }
            var throughModelIncludeOptions = new IncludeOptions<Model, ThroughModel>();
            var IncludeModelOptions = new IncludeOptions< ThroughModel,IncludeModel>();
            includeOptions.ItemID = item?.ID;
            foreach (var throughModelIncludeItem in Include(item,throughModelIncludeOptions))
            {
                foreach (var includeItem in Include(throughModelIncludeItem, IncludeModelOptions))
                {
                    yield return includeItem;
                }
            }
        }
        public IEnumerable<IncludeModel> Include<Model, IncludeModel>(Model item, IncludeOptions<Model, IncludeModel> includeOptions = null, Type type = null) where Model :  IDBModel, new() where IncludeModel :  IDBModel, new()
        {
            if (includeOptions == null)
            {
                includeOptions = new IncludeOptions<Model, IncludeModel>();
            }
            includeOptions.ItemID = item?.ID;
         
            foreach (var includeItem in GetItems(includeOptions))
            {
                yield return includeItem;
            }
        }
        public Model GetItemByID<Model>(long Id, SelectOptions<Model> selectOptions = null) where Model :  IDBModel, new()
        {
            if (selectOptions == null)
            {
                selectOptions = new SelectOptions<Model>();
            }
            if (selectOptions.Where == null)
            {
                selectOptions.Where = new Where();
            }
            selectOptions.Where.AddConditons(WhereConditon.IsEqual("Id", Id));
            selectOptions.Limit = 1;
            return GetItems(selectOptions).FirstOrDefault();
        }
        public IEnumerable<Model> GetItems<Model>(SelectOptions<Model> selectOptions = null ) where Model :  IDBModel, new()
        {
            Type  type = typeof(Model);
            if (selectOptions == null)
            {
                selectOptions = new SelectOptions<Model>();
            }
            System.Reflection.PropertyInfo[] props = type.GetProperties();
            var command = _dataBase.CreateCommand(selectOptions.GetCommand());
            if(selectOptions.Where?.Conditons!=null)
            foreach (var condition in selectOptions.Where.Conditons)
            {
                    foreach (var item in condition)
                    {
                        command.Parameters.Add(new SQLiteParameter(dbType: System.Data.DbType.Object,value:item.Value));
                    }
            }
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                Dictionary<string, object> values = PropertyAttribute.GetPopulateProperties(type, props);
                if (values.Count == 0)
                {
                    throw new AggregateException($"Type {type.FullName} properties have no SQLite attributes");
                }
                Model temp  ;
                System.Reflection.MethodInfo fillMethod = FillMethodAttribute.GetFillMethod(type);
                object[] fillMethodInvArgs = null;
                if (fillMethod != null)
                {
                    fillMethodInvArgs = new object[] { this };
                }
                Dictionary<string, object> keyValues = new Dictionary<string, object>(values);
                while (reader.Read())
                {
                    temp = new Model();
                    if (temp is ModelBase.Model model)
                    {
                        model.SetDataBase(_dataBase);
                    }
                    int ordinal = -1;
                    foreach (KeyValuePair<string, object> item in values)
                    {
                        try
                        {
                            ordinal= reader.GetOrdinal(item.Key);
                            if (ordinal > -1)
                            keyValues[item.Key] = reader.GetValue(ordinal);
                        }
                        catch (System.IndexOutOfRangeException)
                        {
                            if (Debugger.IsAttached)
                                Debug.WriteLine(($"Property '{item.Key}' is Ignored"));
                        }
                    }
                   
                    PropertyAttribute.SetProperties(in temp, props, keyValues);
                    fillMethod?.Invoke(temp, fillMethodInvArgs);
                    yield return temp;
                }
            }
        }

    }
}