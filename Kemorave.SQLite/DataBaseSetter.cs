using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;

using Kemorave.SQLite.ModelBase;
using Kemorave.SQLite.SQLiteAttribute;

namespace Kemorave.SQLite
{
	public class DataSetter : Progress<double>
	{
		public const string InsertOperation = "Insert";
		public const string UpdateOperation = "Update";
		public const string DeleteOperation = "Delete";

		public event EventHandler<IDBModel> BeforeInsert;
		public event EventHandler<IDBModel> AfterInsert;
		public event EventHandler<IDBModel> BeforeUpdate;
		public event EventHandler<IDBModel> AfterUpdate;
		public event EventHandler<IDBModel> BeforeDelete;
		public event EventHandler<IDBModel> AfterDelete;

		public DataSetter(SQLiteDataBase dataBase)
		{
			DataBase = dataBase;
		}

		public DataSetter()
		{
		}

		private bool _isCanceled;

		private readonly SQLiteDataBase DataBase;

		#region Operaion
		public void ToPercentage(long value, long maximume)
		{
			try
			{
				if (value != 0 && maximume != 0)
				{
					OnReport(Math.Round(((value * 100.0) / maximume), 2));
				}
			}
			catch (Exception) { }
		}
		private void CheckCancellation()
		{
			if (_isCanceled)
			{
				throw new OperationCanceledException($"Operation {CurrentOperation} is cancelled");
			}
		}
		private void OnOperationEnd()
		{
			IsBusy = false;
			_isCanceled = false;
			CurrentOperation = null;
			if (AutoCommitChanges)
			{
				CommitChanges();
			}
		}
		internal void CheckBusyState()
		{
			if (IsBusy)
			{
				throw new DatabaseBusyException("Database is busy with " + CurrentOperation + " operation");
			}
			else
			{
				CommitChanges();
			}
		}
		private void OnOperationStart(string operation)
		{
			IsBusy = true;
			CurrentOperation = operation;
			if (operation == DeleteOperation)
			{
				LastTransaction = DataBase.Connection.BeginTransaction(IsolationLevel.ReadCommitted);
			}
			else
			{
				LastTransaction = DataBase.Connection.BeginTransaction(IsolationLevel.Serializable);
			}
		}

		#endregion

		public void CommitChanges()
		{
			try
			{
				LastTransaction?.Commit();
			}
			catch(Exception ex) {
				Debug.WriteLine("Commit Error "+ex);
			}
			finally
			{
				LastTransaction = null;
			}
		}




		public void RollBack()
		{
			DataBase.ExecuteCommand("ROLLBACK");
		}
		public void CancelPendingOperation()
		{
			_isCanceled = true;
		}

		public int Delete<T>(params T[] rows) where T : IDBModel
		{
			if (rows == null)
			{
				throw new ArgumentNullException(nameof(rows));
			}
			if (rows.Length <= 0)
			{
				return -1;
			}
			CheckBusyState();
			lock (this)
			{
				try
				{
					OnOperationStart(DeleteOperation);
					Type type = rows[0].GetType();


					string tableName = TableAttribute.GetTableName(type);


					int TORE = 0;

					using (SQLiteCommand command = DataBase.CreateCommand($"DELETE FROM  {tableName} WHERE Id = ?"))
					{
						foreach (T item in rows)
						{
							command.Parameters.Add(new SQLiteParameter(DbType.Object, item.Id));

							CheckCancellation();

							BeforeDelete?.Invoke(this, item);
							TORE += command.ExecuteNonQuery();
							AfterDelete?.Invoke(this, item);
							command.Parameters.Clear();

							ToPercentage(TORE, rows.Length);
						}
					}
					return TORE;
				}
				finally
				{
					OnOperationEnd();
				}
			}
		}

		public int Insert<T>(T row) where T : IDBModel
		{
			return Insert(new List<T> { row });
		}

		public int Insert<T>(IList<T> rows) where T : IDBModel
		{
			if (rows == null)
			{
				throw new ArgumentNullException(nameof(rows));
			}
			if (rows.Count <= 0)
			{
				return -1;
			}
			CheckBusyState();
			lock (this)
			{
				try
				{
					OnOperationStart(InsertOperation);

					Type type = rows.First().GetType();

					string tableName = TableAttribute.GetTableName(type);

					int TORE = 0;
					System.Reflection.PropertyInfo[] props = type.GetProperties().Where(p => p.CanRead).ToArray();

					Dictionary<string, object> keyValues = PropertyAttribute.GetIncludeProperties(type, props);
					if (keyValues?.Count <= 0)
					{
						throw new AggregateException($"Type ({type.FullName}) properties have no SQLite attributes");
					}
					System.Reflection.MethodInfo fillMethod = FillMethodAttribute.GetFillMethod(type);
					object[] fillMethodInvArgs = null;
					if (fillMethod != null)
					{
						fillMethodInvArgs = new object[] { this };
					}
					Tuple<string, string> tuple = keyValues.GetInsertNamesAndParameters();

					using (SQLiteCommand command = DataBase.CreateCommand($"INSERT INTO [{tableName}] ({tuple.Item1}) VALUES ({tuple.Item2})"))
					{
						foreach (IDBModel item in rows)
						{

							keyValues = PropertyAttribute.GetIncludeProperties(type, props, item);
							foreach (KeyValuePair<string, object> val in keyValues)
							{
								command.Parameters.Add(new SQLiteParameter(DbType.Object, val.Value));
							}
							CheckCancellation();
							BeforeInsert?.Invoke(this, item);
							TORE += command.ExecuteNonQuery();
							item.Id = DataBase.Connection.LastInsertRowId;

							if (item is ModelBase.Model model)
							{
								if (model.isNew)
								{
									model.SetDataBase(DataBase);
								}
							}
							fillMethod?.Invoke(item, fillMethodInvArgs);
							AfterInsert?.Invoke(this, item);
							command.Parameters.Clear();

							ToPercentage(TORE, rows.Count);
						}
					}
					return TORE;
				}
				finally
				{
					OnOperationEnd();
				}
			}
		}
		public int Update<T>(params T[] rows) where T : IDBModel
		{
			if (rows == null)
			{
				throw new ArgumentNullException(nameof(rows));
			}
			if (rows.Length <= 0)
			{
				return -1;
			}
			CheckBusyState();
			lock (this)
			{
				try
				{
					OnOperationStart(UpdateOperation);
					Type type = rows[0].GetType();

					string tableName = TableAttribute.GetTableName(type);


					int TORE = 0;


					System.Reflection.PropertyInfo[] props = type.GetProperties();
					Dictionary<string, object> keyValues = PropertyAttribute.GetIncludeProperties(type, props);
					if (keyValues?.Count <= 0)
					{
						throw new AggregateException($"Type {type.FullName} properties have no SQLite attributes");
					}
					string parameters = keyValues.GetUpdateParameters();

					using (SQLiteCommand command = DataBase.CreateCommand($"UPDATE {tableName} SET {parameters} WHERE Id = ?"))
					{
						foreach (IDBModel item in rows)
						{
							keyValues = PropertyAttribute.GetIncludeProperties(type, props, item);
							foreach (KeyValuePair<string, object> val in keyValues)
							{
								command.Parameters.Add(new SQLiteParameter(DbType.Object, val.Value));
							}
							CheckCancellation();
							command.Parameters.Add(new SQLiteParameter(DbType.Object, item.Id));

							BeforeUpdate?.Invoke(this, item);
							TORE += command.ExecuteNonQuery();
							AfterUpdate?.Invoke(this, item);
							command.Parameters.Clear();

							ToPercentage(TORE, rows.Length);
						}
					}
					return TORE;
				}
				finally
				{
					OnOperationEnd();
				}
			}
		}

		/// <summary>
		///  When true database commits changes without a chance for rollbacks,
		///  used with <see cref="IDataBaseSetter.CommitChanges"/> 
		///  <para/>
		///  Default is true
		/// </summary>

		public bool AutoCommitChanges { get; set; } = true;
		public volatile bool IsBusy;
		public bool CanRollBack => LastTransaction != null;
		public string CurrentOperation { get; protected set; }
		protected SQLiteTransaction LastTransaction { get; set; }

	}
}