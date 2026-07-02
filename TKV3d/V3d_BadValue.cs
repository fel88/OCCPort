using System.Runtime.Serialization;

namespace TKV3d
{
    [Serializable]
    public class V3d_BadValue : Exception
    {
        public V3d_BadValue()
        {
        }

        public V3d_BadValue(string message) : base(message)
        {
        }

        public V3d_BadValue(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected V3d_BadValue(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

