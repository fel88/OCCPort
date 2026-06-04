using System.Runtime.Serialization;

namespace OCCPort.Common
{
    [Serializable]
    public class Standard_Failure : Exception
    {
        public Standard_Failure()
        {
        }

        public Standard_Failure(string message) : base(message)
        {
        }

        public Standard_Failure(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected Standard_Failure(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        internal static void Raise(string v)
        {
            throw new Exception(v);
        }
    }
}
