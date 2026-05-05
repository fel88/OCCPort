using System;
using System.Runtime.Serialization;

namespace OCCPort
{
    [Serializable]
    internal class Standard_NotImplemented : NotImplementedException
    {
        public Standard_NotImplemented()
        {
        }

        public Standard_NotImplemented(string message) : base(message)
        {
        }

        public Standard_NotImplemented(string message, Exception innerException) : base(message, innerException)
        {
        }

        
    }
}