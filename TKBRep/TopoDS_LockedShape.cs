using System;
using System.Runtime.Serialization;

namespace OCCPort
{
    [Serializable]
    internal class TopoDS_LockedShape : Exception
    {
        public TopoDS_LockedShape()
        {
        }

        public TopoDS_LockedShape(string message) : base(message)
        {
        }

        public TopoDS_LockedShape(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TopoDS_LockedShape(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}