using System;

namespace OCCPort
{
    [Serializable]
    internal class Standard_DimensionError : Exception
    {
        public Standard_DimensionError()
        {
        }

        public Standard_DimensionError(string message) : base(message)
        {
        }

        public Standard_DimensionError(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}