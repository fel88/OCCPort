using System.Runtime.Serialization;

namespace OCCPort.Common
{
    [Serializable]
    public class StdFail_NotDone : Exception
    {
        public StdFail_NotDone()
        {
        }

        public StdFail_NotDone(string message) : base(message)
        {
        }

        public StdFail_NotDone(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected StdFail_NotDone(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
