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
        void CommitChanges();
        int Delete<T>(IList<T> rows, string tableName = null) where T : class, IDBModel;
        int Delete<T>(T item, string tableName = null) where T : class, IDBModel, new();
        int Insert<T>(IList<T> rows, string tableName = null) where T : class, IDBModel;
        int Insert<T>(T item, string tableName = null) where T : class, IDBModel;
        void RollBack();
        int Update<T>(IList<T> rows, string tableName = null) where T : class, IDBModel;
        int Update<T>(T item, string tableName = null) where T : class, IDBModel, new();
    }
}