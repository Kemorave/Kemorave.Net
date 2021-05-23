using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using Kemorave.SQLite.Options;
using Kemorave.SQLite.SQLiteAttribute;

namespace Kemorave.SQLite.ModelBase
{
    public  class Model : IDBModel
    {

        public Model()
        {

            isNew = true;
        }

        public Model(SQLiteDataBase dataBase):this()
        {
            DataBase = dataBase; 
        }

        ~Model()
        {
            if(DataBase!=null)
            DataBase.Connection.Update -= Connection_Update;
        }
        
        //////////////////////////////////////////////////////////////////////////////////////////
     
        internal void SetDataBase(SQLiteDataBase dataBase)
        { 
            DataBase = dataBase;
            isNew = false;
            DataBase.Connection.Update -= Connection_Update;
            DataBase.Connection.Update += Connection_Update;
            OnLoad(dataBase);
        }
        private void Connection_Update(object sender, UpdateEventArgs e)
        {
            if (e.RowId==Id&&e.Table.Equals(TableAttribute.GetTableName(GetType()), StringComparison.Ordinal))
            {
                switch (e.Event)
                {
                    case UpdateEventType.Delete:
                        OnDelete(); break;
                    case UpdateEventType.Update:
                        if (isSource)
                        { isSource = false;
                            return;
                        }
                        OnUpdate(); break;
                }
            }
        }
       

        protected virtual void OnDelete()
        {

        }
        protected virtual void OnUpdate()
        {
            Reload();
        }
        protected virtual void OnLoad(SQLiteDataBase dataBase)
        {

        }
        public virtual void Delete()
        {
            if (!isNew)
            {
                DataBase?.DataSetter?.Delete(this);
            }
        }
        public virtual  void Reload() {
            DataBase?.DataGetter?.ReloadItems(this);
        }
        public virtual void Save()
        {
            isSource = true;
            if (isNew)
            {
                DataBase?.DataSetter?.Insert(this);
                isNew = false;
            }
            else
            DataBase?.DataSetter?.Update(this);
           
        }

        //////////////////////////////////////////////////////////////////////////////////////////

        protected SQLiteDataBase DataBase;
        [Property(Behavior.Populate)]
        [TableColumn("Id", SQLiteType.INTEGER, true, true, false)]
        public long Id { get; set; }
        public bool isNew = false;bool isSource=false;
    }
}