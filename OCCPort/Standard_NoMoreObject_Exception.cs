using System;
using System.Runtime.Serialization;

namespace OCCPort
{
    [Serializable]
    internal class Standard_NoMoreObject_Exception : Exception
    {
        public Standard_NoMoreObject_Exception()
        {
        }

        public Standard_NoMoreObject_Exception(string message) : base(message)
        {
        }

        public Standard_NoMoreObject_Exception(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected Standard_NoMoreObject_Exception(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}