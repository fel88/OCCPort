using System;
using System.Runtime.Serialization;

namespace OCCPort
{
    [Serializable]
    internal class Standard_DomainError : Exception
    {
        public Standard_DomainError()
        {
        }

        public Standard_DomainError(string message) : base(message)
        {
        }

        public Standard_DomainError(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected Standard_DomainError(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}