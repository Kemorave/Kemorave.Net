using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using Kemorave.SQLite.Options;
using Kemorave.SQLite.SQLiteAttribute;

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

        public IEnumerable<IncludeModel> IncludeThrough<Model, IncludeModel, ThroughModel>(Model item, IncludeOptions<Model, IncludeModel> includeOptions = null) where Model : ModelBase.Model, IDBModel, new() where IncludeModel : IDBModel, new() where ThroughModel : IDBModel, new()
        {
            if (includeOptions == null)
            {
                includeOptions = new IncludeOptions<Model, IncludeModel>();

            }
            IncludeOptions<Model, ThroughModel> throughModelIncludeOptions = new IncludeOptions<Model, ThroughModel>();
            IncludeOptions<ThroughModel, IncludeModel> IncludeModelOptions = new IncludeOptions<ThroughModel, IncludeModel>();
            includeOptions.ItemID = item?.Id;
            foreach (ThroughModel throughModelIncludeItem in Include(item, throughModelIncludeOptions))
            {
                foreach (IncludeModel includeItem in Include(throughModelIncludeItem, IncludeModelOptions))
                {
                    yield return includeItem;
                }
            }
        }
        public IEnumerable<IncludeModel> Include<Model, IncludeModel>(Model item, IncludeOptions<Model, IncludeModel> includeOptions = null) where Model : IDBModel, new() where IncludeModel : IDBModel, new()
        {
            if (includeOptions == null)
            {
                includeOptions = new IncludeOptions<Model, IncludeModel>();
            }
            includeOptions.ItemID = item?.Id;

            foreach (IncludeModel includeItem in GetItems(includeOptions))
            {
                yield return includeItem;
            }
        }
        public Model GetItemByID<Model>(long Id, SelectOptions<Model> selectOptions = null) where Model : IDBModel, new()
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
        public IEnumerable<Model> GetItems<Model>(SelectOptions<Model> selectOptions = null) where Model : IDBModel, new()
        {
            Type type = typeof(Model);
            if (selectOptions == null)
            {
                selectOptions = new SelectOptions<Model>();
            }
            System.Reflection.PropertyInfo[] props = type.GetProperties();

            using (SQLiteCommand command = _dataBase.CreateCommand(selectOptions))
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                Dictionary<string, object> values = PropertyAttribute.GetPopulateProperties(type, props);
                if (values.Count == 0)
                {
                    throw new AggregateException($"Type {type.FullName} properties have no SQLite attributes");
                }
                Model temp;
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
                   
                    int ordinal = -1;
                    foreach (KeyValuePair<string, object> item in values)
                    {
                        try
                        {
                            ordinal = reader.GetOrdinal(item.Key);
                            if (ordinal > -1)
                            {
                                keyValues[item.Key] = reader.GetValue(ordinal);
                            }
                        }
                        catch (IndexOutOfRangeException)
                        {
                            if (Debugger.IsAttached)
                            {
                                Debug.WriteLine(($"Property '{item.Key}' is Ignored"));
                            }
                        }
                    }

                    PropertyAttribute.SetProperties(in temp, props, keyValues);
                    fillMethod?.Invoke(temp, fillMethodInvArgs);
                     if (temp is ModelBase.Model model)
                    {
                        model.SetDataBase(_dataBase);
                    }
                    yield return temp;
                }
            }
        }

        public void ReloadItems<Model>(params Model[] models) where Model : IDBModel, new()
        {
            if (models == null)
            {
                throw new ArgumentNullException(nameof(models));
            }
            if (models.Length == 0)
            {
                return;
            }
            Type type = models[0].GetType();
            string tbName = TableAttribute.GetTableName(type);
            System.Reflection.PropertyInfo[] props = type.GetProperties();
            Dictionary<string, object> populateProp = PropertyAttribute.GetPopulateProperties(type, props);
            var ids = models.Select(m => m.Id as object).ToArray(); 
            var op = new SelectOptions<Model>(null, null, new Where(WhereConditon.IsIn("Id", ids))) { Table = tbName };
            op.OrderBy = "Id";
            using (SQLiteCommand command = _dataBase.CreateCommand(op))
            using (SQLiteDataReader reader = command.ExecuteReader())
            {

                if (populateProp.Count == 0)
                {
                    throw new AggregateException($"Type {type.FullName} properties have no SQLite attributes");
                }

                Dictionary<string, object> keyValues = new Dictionary<string, object>(populateProp);
                while (reader.Read())
                { 
                    int ordinal = -1;
                    foreach (var tmp in models.OrderBy(m => m.Id))
                    {
                        foreach (KeyValuePair<string, object> property in populateProp)
                        {
                            try
                            {
                                ordinal = reader.GetOrdinal(property.Key);
                                if (ordinal > -1)
                                {
                                    keyValues[property.Key] = reader.GetValue(ordinal);
                                }
                            }
                            catch (IndexOutOfRangeException)
                            {
                                if (Debugger.IsAttached)
                                {
                                    Debug.WriteLine(($"Property '{property.Key}' is Ignored"));
                                }
                            }
                        } 
                        PropertyAttribute.SetProperties(in tmp, props, keyValues);
                    }


                }
            }
        }

    }
}