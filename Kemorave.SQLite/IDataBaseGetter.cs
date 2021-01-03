using System.Collections.Generic;

namespace Kemorave.SQLite
{
    public interface IDataBaseGetter
    {
        T GetItemByID<T>(long id, string tableName = null) where T : class, IDBModel, new();
        IEnumerable<T> GetItems<T>(string tableName = null, string selection = "*", string condition = null) where T : class, IDBModel, new();
    }
}