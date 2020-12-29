using System;
using System.Collections.Generic;
using System.Text;

namespace Kemorave.Net.Api.DB
{
    public class DBResult : EventArgs
    {
        public DBResult(string actionCode)
        {
            ActionCode = actionCode ?? throw new ArgumentNullException(nameof(actionCode));
        }

        public DBResult(string actionCode, IDBModel affectedItem) : this(actionCode)
        {
            AffectedItem = affectedItem ?? throw new ArgumentNullException(nameof(affectedItem));
        }

        public DBResult(string actionCode, Exception exception) : this(actionCode)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }

        public DBResult(string actionCode, Exception exception, IDBModel affectedItem) : this(actionCode)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            AffectedItem = affectedItem ?? throw new ArgumentNullException(nameof(affectedItem));
        }

        public object Tag { get; set; }
        public string ActionCode { get; }
        public bool HasError { get => Exception != null; }
        public Exception Exception { get; }
        public IDBModel AffectedItem { get; }
    }
}