using System;
using System.Runtime.Serialization;

namespace Kemorave.SQLite
{
    [Serializable]
    internal class PropertyNotFound : Exception
    {
        public PropertyNotFound()
        {
        }

        public PropertyNotFound(string message) : base(message)
        {
        }

        public PropertyNotFound(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PropertyNotFound(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}