using System;
using System.Runtime.Serialization;

namespace Kemorave.Net
{
    [Serializable]
    internal class ResponceExeption : Exception
    { 

        public ResponceExeption()
        {
        }

        public ResponceExeption(ResponceMessage message):this(message.Message)
        {
            this.ResponceMessage = message;
        }

        public ResponceExeption(string message) : base(message)
        {
        }

        public ResponceExeption(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ResponceExeption(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ResponceMessage ResponceMessage { get; }
        public override string ToString()
        {
            return ResponceMessage.ToString();
        }
    }
}