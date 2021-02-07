using System;
using System.Collections.Generic;

namespace Kemorave.SQLite
{
    public interface IDataBaseSetter
    {
        bool AutoCommitChanges { get; set; }
        bool CanRollBack { get; }
        string CurrentOperation { get; }
        bool IsBusy { get; }

        void CancelPendingOperation();
        int ClearTable(string tableName);
        void CommitChanges();
        int CreateTable(TableInfo table);
        int CreateTable(Type type);
        int Delete<T>(IList<T> rows, string tableName = null) where T : class, IDBModel;
        int Delete<T>(T item, string tableName = null) where T : class, IDBModel, new();
        int DropTable(string tableName);
        int Insert<T>(IList<T> rows, string tableName = null) where T : class, IDBModel;
        int Insert<T>(T item, string tableName = null) where T : class, IDBModel;
        int Recreate(DataBaseInfo dBInfo, bool force = false);
        void RollBack();
        int Update<T>(IList<T> rows, string tableName = null) where T : class, IDBModel;
        int Update<T>(T item, string tableName = null) where T : class, IDBModel, new();
    }
}