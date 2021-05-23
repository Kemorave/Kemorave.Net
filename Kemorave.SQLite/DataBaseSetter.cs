using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using Kemorave.SQLite.SQLiteAttribute;

namespace Kemorave.SQLite
{
    public class DataBaseSetter : Progress<double>
    {
        public const string InsertOperation = "Insert";
        public const string UpdateOperation = "Update";
        public const string DeleteOperation = "Delete";
        public DataBaseSetter(SQLiteDataBase dataBase)
        {
            DataBase = dataBase;
        }

        public DataBaseSetter()
        {
        }

        private bool _isCanceled;

        private readonly SQLiteDataBase DataBase;

        #region Operaion
        public  void ToPercentage(long value, long maximume)
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
        private  void OnOperationEnd()
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
                throw new DatabaseBusyException("Database is busy with" + CurrentOperation + " operation");
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
            LastTransaction = DataBase.Connection.BeginTransaction(IsolationLevel.Serializable);
        }

        #endregion

        public void CommitChanges()
        {
            LastTransaction?.Commit();
            LastTransaction = null;
        }




        public void RollBack()
        {
            DataBase.ExecuteCommand("ROLLBACK");
        }
        public void CancelPendingOperation()
        {
            _isCanceled = true;
        }

        public int Delete<T>(params T[] rows) where T :  IDBModel
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
            try
            {
                OnOperationStart(DeleteOperation);
                Type type = rows[0].GetType();

                
                 string   tableName = TableAttribute.GetTableName(type);
                 

                int TORE = 0;

                using (SQLiteCommand command = DataBase.CreateCommand($"DELETE FROM  {tableName} WHERE Id = ?"))
                {
                    foreach (T item in rows)
                    {
                        command.Parameters.Add(new SQLiteParameter(DbType.Object, item.Id));

                        CheckCancellation();
                        TORE += command.ExecuteNonQuery();
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
        public int Insert<T>(params T[] rows) where T :  IDBModel
        {
            if (rows == null)
            {
                throw new ArgumentNullException(nameof(rows));
            }
            if (rows.Length<=0)
            {
                return -1;
            }
            CheckBusyState();
            try
            {
                OnOperationStart(InsertOperation);

                Type type = rows[0].GetType();
               
                 string   tableName = TableAttribute.GetTableName(type);
                

                int TORE = 0;
                System.Reflection.PropertyInfo[] props = type.GetProperties().Where(p=>p.CanRead).ToArray();

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
                        TORE += command.ExecuteNonQuery();
                        item.Id = DataBase.Connection.LastInsertRowId;
                         if (item is ModelBase.Model model)
                        {
                            if(model.isNew)
                            model.SetDataBase(DataBase);
                        }fillMethod?.Invoke(item, fillMethodInvArgs);
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
        public int Update<T>(params T[] rows) where T :  IDBModel
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
            try
            {
                OnOperationStart(UpdateOperation);
                Type type = rows[0].GetType();
               
                 string   tableName = TableAttribute.GetTableName(type);
                

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
                    foreach (T item in rows)
                    {
                        keyValues = PropertyAttribute.GetIncludeProperties(type, props, item);
                        foreach (KeyValuePair<string, object> val in keyValues)
                        {
                            command.Parameters.Add(new SQLiteParameter(DbType.Object, val.Value));
                        }
                        CheckCancellation();
                        command.Parameters.Add(new SQLiteParameter(DbType.Object, item.Id));
                        TORE += command.ExecuteNonQuery();
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

        /// <summary>
        ///  When true database commits changes without a chance for rollbacks,
        ///  used with <see cref="IDataBaseSetter.CommitChanges"/> 
        ///  <para/>
        ///  Default is true
        /// </summary>

        public bool AutoCommitChanges { get; set; } = true;
        public bool IsBusy { get; protected set; }
        public bool CanRollBack => LastTransaction != null;
        public string CurrentOperation { get; protected set; }
        protected SQLiteTransaction LastTransaction { get; set; }

    }
}