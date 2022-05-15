using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;

using Kemorave.SQLite.ModelBase;
using Kemorave.SQLite.Options;
using Kemorave.SQLite.SQLiteAttribute;

namespace Kemorave.SQLite
{
	public class DataGetter
	{

		public DataGetter(SQLiteDataBase dataBase)
		{
			DataBase = dataBase ?? throw new ArgumentNullException(nameof(dataBase));
		}

		public System.Data.DataTable GetDataTable(string cmd)
		{
			using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(cmd, DataBase.Connection))
			{
				System.Data.DataTable dataTable = new System.Data.DataTable();
				dataAdapter.Fill(dataTable);
				return dataTable;
			}
		}

		public IEnumerable<IncludeModel> FindIncluded<Model, IncludeModel, ThroughModel>(Model item, IncludeOptions<Model, IncludeModel> includeOptions = null) where Model : ModelBase.Model, IDBModel, new() where IncludeModel : IDBModel, new() where ThroughModel : IDBModel, new()
		{
			if (includeOptions == null)
			{
				includeOptions = new IncludeOptions<Model, IncludeModel>();
			}

			IncludeOptions<Model, ThroughModel> throughModelIncludeOptions = new IncludeOptions<Model, ThroughModel>();
			IncludeOptions<ThroughModel, IncludeModel> IncludeModelOptions = new IncludeOptions<ThroughModel, IncludeModel>();
			
			includeOptions.ItemID = item?.Id;
			
			foreach (ThroughModel throughModelIncludeItem in FindIncluded(item, throughModelIncludeOptions))
			{
				foreach (IncludeModel includeItem in FindIncluded(throughModelIncludeItem, IncludeModelOptions))
				{
					yield return includeItem;
				}
			}
		}

		public IEnumerable<IncludeModel> FindIncluded<Model, IncludeModel>(Model item, IncludeOptions<Model, IncludeModel> includeOptions = null) where Model : IDBModel, new() where IncludeModel : IDBModel, new()
		{
			if (includeOptions == null)
			{
				includeOptions = new IncludeOptions<Model, IncludeModel>();
			}

			includeOptions.ItemID = item?.Id;

			foreach (IncludeModel includeItem in FindAll(includeOptions))
			{
				yield return includeItem;
			}
		}

		public Model FindOneById<Model>(long? Id, SelectOptions<Model> selectOptions = null) where Model : IDBModel, new()
		{
			if (Id == null && selectOptions == null)
			{
				throw new ArgumentNullException("Id and select options are null");
			}
			if (selectOptions == null)
			{
				selectOptions = new SelectOptions<Model>();
			}
			if (selectOptions.Where == null)
			{
				selectOptions.Where = new Where();
			}
			if (Id != null)
			{
				selectOptions.Where.AddConditons(WhereConditon.IsEqual("Id", Id));
			}

			selectOptions.Limit = 1;
			return FindAll(selectOptions).FirstOrDefault();
		}

		public Model LastRowInserted<Model>() where Model : IDBModel, new()
		{
			return FindAll<Model>(new SelectOptions<Model>(" id DESC ") { Limit = 1 }).FirstOrDefault();
		}

		public Model FirstRowInserted<Model>() where Model : IDBModel, new()
		{
			return FindAll<Model>(new SelectOptions<Model>(" id ") { Limit = 1 }).FirstOrDefault();
		}

		public IEnumerable<Model> FindAll<Model>(SelectOptions<Model> selectOptions = null) where Model : IDBModel, new()
		{
			Type type = typeof(Model);
			if (selectOptions == null)
			{
				selectOptions = new SelectOptions<Model>();
			}
			System.Reflection.PropertyInfo[] props = type.GetProperties();

			using (SQLiteCommand command = DataBase.CreateCommand(selectOptions))
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
								Debug.WriteLine(($"Warning Property '{item.Key}' is Ignored"));
							}
						}
					}

					PropertyAttribute.SetProperties(in temp, props, keyValues);

					fillMethod?.Invoke(temp, fillMethodInvArgs);
					(temp as ModelBase.Model)?.SetDataBase(DataBase);

					yield return temp;
				}
			}
		}

		public void Reload<Model>(params Model[] models) where Model : IDBModel, new()
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
			object[] ids = models.Select(m => m.Id as object).ToArray();
			SelectOptions<Model> op = new SelectOptions<Model>(null, null, new Where(WhereConditon.IsIn("Id", ids))) { Table = tbName };
			op.OrderBy = "Id";
			using (SQLiteCommand command = DataBase.CreateCommand(op))
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
					foreach (Model tmp in models.OrderBy(m => m.Id))
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

		public SQLiteDataBase DataBase { get; }

	}
}