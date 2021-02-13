using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Kemorave.SQLite.Attribute;

namespace Kemorave.SQLite
{
    internal class DataBaseSetter : IDataBaseSetter
    {
        public const string InsertOperation = "Insert";
        public const string UpdateOperation = "Update";
        public const string DeleteOperation = "Delete";
        public DataBaseSetter(SQLiteDataBase dataBase)
        {
            DataBase = dataBase;
        }
        private bool _isCanceled;

        private readonly SQLiteDataBase DataBase;

        #region Operaion
        private void CheckParams<T>(string tableName, IList<T> rows) where T : class, IDBModel
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException(nameof(tableName));
            }
            if (rows == null)
            {
                throw new ArgumentNullException($"{nameof(rows)} can't be empty or null");
            }
            if (rows.Count == 0)
            {
                throw new InvalidOperationException("Items list has no items");
            }
        }
        private void CheckCancellation()
        {
            if (_isCanceled)
            {
                throw new OperationCanceledException("Operation " + CurrentOperation + " is cancelled");
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
            LastTransaction = DataBase.Connection.BeginTransaction(System.Data.IsolationLevel.Serializable);
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



        public int Delete<T>(IList<T> rows, string tableName = null) where T : class, IDBModel
        {
            CheckBusyState();
            try
            {
                OnOperationStart(DeleteOperation);
                Type type = typeof(T);

                if (tableName == null)
                {
                    tableName = SQLiteTableAttribute.GetTableName(type);
                }
                CheckParams(tableName, rows);

                int TORE = 0;

                using (SQLiteCommand command = DataBase.CreateCommand($"DELETE FROM  {tableName} WHERE ID = ?"))
                {
                    foreach (T item in rows)
                    {
                        command.Parameters.Add(new SQLiteParameter(DbType.Object, item.ID));

                        CheckCancellation();
                        TORE += command.ExecuteNonQuery();
                        command.Parameters.Clear();
                    }
                }
                return TORE;
            }
            finally
            {
                OnOperationEnd();
            }
        }
        public int Insert<T>(IList<T> rows, string tableName = null) where T : class, IDBModel
        {
            CheckBusyState();
            try
            {
                OnOperationStart(InsertOperation);

                Type type = typeof(T);
                if (tableName == null)
                {
                    tableName = SQLiteTableAttribute.GetTableName(type);
                }
                CheckParams(tableName, rows);

                int TORE = 0;
                System.Reflection.PropertyInfo[] props = type.GetProperties();

                Dictionary<string, object> keyValues = SQLitePropertyAttribute.GetIncludeProperties(type, props);
                if (keyValues?.Count <= 0)
                {
                    throw new AggregateException($"Type {type.FullName} properties have no SQLite attributes");
                }
                //System.Reflection.MethodInfo fillMethod = SQLiteFillMethodAttribute.GetFillMethod(type);
                //object[] fillMethodInvArgs = null;
                //if (fillMethod != null)
                //{
                //    fillMethodInvArgs = new object[] { this };
                //}
                Tuple<string, string> tuple = keyValues.GetInsertNamesAndParameters();

                using (SQLiteCommand command = DataBase.CreateCommand($"INSERT INTO [{tableName}] ({tuple.Item1}) VALUES ({tuple.Item2})"))
                {
                    foreach (T item in rows)
                    {
                        keyValues = SQLitePropertyAttribute.GetIncludeProperties(type, props, item);
                        foreach (KeyValuePair<string, object> val in keyValues)
                        {
                            command.Parameters.Add(new SQLiteParameter(DbType.Object, val.Value));
                        }
                        CheckCancellation();
                        TORE += command.ExecuteNonQuery();
                        item.ID = DataBase.Connection.LastInsertRowId;
                        //fillMethod?.Invoke(item, fillMethodInvArgs);
                        command.Parameters.Clear();
                    }
                }
                return TORE;
            }
            finally
            {
                OnOperationEnd();
            }
        }
        public int Update<T>(IList<T> rows, string tableName = null) where T : class, IDBModel
        {
            CheckBusyState();
            try
            {
                OnOperationStart(UpdateOperation);
                Type type = typeof(T);
                if (tableName == null)
                {
                    tableName = SQLiteTableAttribute.GetTableName(type);
                }
                CheckParams<T>(tableName, rows);

                int TORE = 0;


                System.Reflection.PropertyInfo[] props = type.GetProperties();
                Dictionary<string, object> keyValues = SQLitePropertyAttribute.GetIncludeProperties(type, props);
                if (keyValues?.Count <= 0)
                {
                    throw new AggregateException($"Type {type.FullName} properties have no SQLite attributes");
                }
                string parameters = keyValues.GetUpdateParameters();

                using (SQLiteCommand command = DataBase.CreateCommand($"UPDATE {tableName} SET {parameters} WHERE ID = ?"))
                {
                    foreach (T item in rows)
                    {
                        keyValues = SQLitePropertyAttribute.GetIncludeProperties(type, props, item);
                        foreach (KeyValuePair<string, object> val in keyValues)
                        {
                            command.Parameters.Add(new SQLiteParameter(DbType.Object, val.Value));
                        }
                        CheckCancellation();
                        command.Parameters.Add(new SQLiteParameter(DbType.Object, item.ID));
                        TORE += command.ExecuteNonQuery();
                        command.Parameters.Clear();
                    }
                }
                return TORE;
            }
            finally
            {
                OnOperationEnd();
            }
        }

        public int Insert<T>(T item, string tableName = null) where T : class, IDBModel
        {
            if (item == null)
            {
                throw new ArgumentNullException($"{nameof(item)} can't be empty");
            }
            return Insert<T>(new List<T>() { item }, tableName);
        }
        public int Update<T>(T item, string tableName = null) where T : class, IDBModel, new()
        {
            if (item == null)
            {
                throw new ArgumentNullException($"{nameof(item)} can't be empty");
            }
            return Update<T>(new List<T>() { item }, tableName);
        }
        public int Delete<T>(T item, string tableName = null) where T : class, IDBModel, new()
        {
            if (item == null)
            {
                throw new ArgumentNullException($"{nameof(item)} can't be empty");
            }
            return Delete<T>(new List<T>() { item }, tableName);
        }
        /// <summary>
        ///  When true database commits changes without a chance for rollbacks 
        ///  else use <see cref="SQLiteDataBase.CommitChanges"/> 
        ///  <para/>
        ///  Default is true
        /// </summary>

        public bool AutoCommitChanges { get; set; } = true;
        public bool IsBusy { get; protected set; }
        public bool CanRollBack => LastTransaction != null;
        public string CurrentOperation { get; protected set; }
        protected System.Data.SQLite.SQLiteTransaction LastTransaction { get; set; }

    }
}