using System;
using System.Collections.Generic;
using System.Text;

namespace Kemorave.Net.Api.DB
{
    public class DBActionResult : EventArgs
    {
        public DBActionResult(string actionCode)
        {
            ActionCode = actionCode ?? throw new ArgumentNullException(nameof(actionCode));
        }

        public DBActionResult(string actionCode, IDBModel affectedItem) : this(actionCode)
        {
            AffectedItem = affectedItem ?? throw new ArgumentNullException(nameof(affectedItem));
        }

        public DBActionResult(string actionCode, Exception exception) : this(actionCode)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            HasError = true;
        }

        public DBActionResult(string actionCode, int affectedItemID) : this(actionCode)
        {
            AffectedItemID = affectedItemID;
        }

        public DBActionResult(string actionCode, Exception exception, IDBModel affectedItem) : this(actionCode, exception)
        {
            AffectedItem = affectedItem ?? throw new ArgumentNullException(nameof(affectedItem));
        }
        public static DBActionResult GetDBResult(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DBActionResult>(json);
        }
        public object Tag { get; set; }
        public string Message { get; set; }
        public string ActionCode { get; set; }
        public bool HasError { get; set; }
        public Exception Exception { get; set; }
        public IDBModel AffectedItem { get; set; }
        public int AffectedItemID { get; set; }
    }
}