using System.Runtime.Serialization;

namespace OCCPort.Common
{
    [Serializable]
    public class Standard_NullObject : Exception
    {
        public Standard_NullObject()
        {
        }

        public Standard_NullObject(string message) : base(message)
        {
        }

        public Standard_NullObject(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected Standard_NullObject(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
