﻿using System;
using System.Data.SQLite;

using Kemorave.SQLite.SQLiteAttribute;

namespace Kemorave.SQLite.ModelBase
{
	public class Model : IDBModel, System.IDisposable
	{

		public Model()
		{
		}

		public Model(SQLiteDataBase dataBase) : this()
		{
			SetDataBase(dataBase);
		}

		~Model()
		{
			Dispose();
		}


		internal void SetDataBase(SQLiteDataBase dataBase)
		{
			if (dataBase == null)
			{
				return;
			}
			DataBase = dataBase;
			isNew = false;
			DataBase.Connection.Update -= Connection_Update;
			DataBase.Connection.Update += Connection_Update;
			OnLoad(dataBase);
		}

		private void Connection_Update(object sender, UpdateEventArgs e)
		{
			if (e.RowId == Id && e.Table.Equals(TableAttribute.GetTableName(GetType()), StringComparison.Ordinal))
			{
				switch (e.Event)
				{
					case UpdateEventType.Update:
						if (isSource)
						{
							isSource = false;
							return;
						}
						Reload(); break;
				}
			}
		}

		protected virtual void OnLoad(SQLiteDataBase dataBase)
		{
			isNew = false;
			this.DataBase = dataBase;
		}
		protected virtual void OnDelete() { }
		protected virtual void OnUpdate() { }
		protected virtual void OnInsert() { }


		public virtual void Delete()
		{
			if (!isNew)
			{
				OnDelete();
				DataBase?.Setter?.Delete(this);
			}
		}
		public virtual void Save()
		{
			isSource = true;
			if (isNew)
			{
				DataBase?.Setter?.Insert(this);
				isNew = false;
				OnInsert();
			}
			else
			{

				DataBase?.Setter?.Update(this);
				OnUpdate();
			}
		}

		public virtual void Reload()
		{
			if (isSource)
			{
				return;
			}
			DataBase?.Getter?.Reload(this);
		}

		public void Dispose()
		{
			if (DataBase != null)
			{
				DataBase.Connection.Update -= Connection_Update;
			}
		}

		protected SQLiteDataBase DataBase;
		[Property(SqlPropertyHandling.Populate)]
		[TableColumn("Id", SQLiteType.INTEGER, true, true, false)]
		public virtual long Id { get; set; }

		public bool isNew = true;
		private bool isSource = false;



	}
}