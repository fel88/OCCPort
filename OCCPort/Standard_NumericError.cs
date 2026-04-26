using System;

namespace OCCPort
{
    [Serializable]
    internal class Standard_NumericError : Exception
    {
        public Standard_NumericError()
        {
        }

        public Standard_NumericError(string message) : base(message)
        {
        }

        public Standard_NumericError(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}