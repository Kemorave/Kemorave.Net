using System;
using System.Data.SQLite;
using Kemorave.SQLite.SQLiteAttribute;

namespace Kemorave.SQLite.ModelBase
{
    public  class Model : IDBModel
    {

        public Model()
        {
        }
       
        ~Model()
        {
            DataBase.Connection.Update -= Connection_Update;
        }
        
        //////////////////////////////////////////////////////////////////////////////////////////
     
        internal void SetDataBase(SQLiteDataBase dataBase)
        { 
            DataBase = dataBase;
            DataBase.Connection.Update += Connection_Update;
            OnLoad(dataBase);
        }
        private void Connection_Update(object sender, System.Data.SQLite.UpdateEventArgs e)
        {
            if (e.RowId==ID&&e.Table.Equals(TableAttribute.GetTableName(GetType()), StringComparison.Ordinal))
            {
                switch (e.Event)
                {
                    case UpdateEventType.Delete:
                        OnDelete(); break;
                    case UpdateEventType.Update:
                        OnUpdate(); break;
                }
            }
        }

        protected virtual void OnDelete()
        {

        }
        protected virtual void OnUpdate()
        {

        }
        protected virtual void OnLoad(SQLiteDataBase dataBase)
        {

        }
        public virtual void Save()
        {
            DataBase.DataSetter.Update(this);
        }

        //////////////////////////////////////////////////////////////////////////////////////////

        protected SQLiteDataBase DataBase;
        [SQLiteAttribute.Property(PropertyAttribute.DefaultValueBehavior.Populate)]
        [TableColumn("ID", SQLiteType.INTEGER, true, true, false)]
        public long ID { get; set; }

    }
}