using System;
using System.Runtime.Serialization;

namespace Kemorave.SQLite
{
    [Serializable]
    internal class DatabaseBusyException : Exception
    {
        public DatabaseBusyException()
        {
        }

        public DatabaseBusyException(string message) : base(message)
        {
        }

        public DatabaseBusyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DatabaseBusyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}