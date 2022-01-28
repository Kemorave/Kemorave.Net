using System;
using System.Runtime.Serialization;

namespace Kemorave.SQLite
{
    [Serializable]
    internal class ForeignKeyNotFoundException : Exception
    {
        public ForeignKeyNotFoundException()
        {
        }

        public ForeignKeyNotFoundException(string message) : base(message)
        {
        }

        public ForeignKeyNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ForeignKeyNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}