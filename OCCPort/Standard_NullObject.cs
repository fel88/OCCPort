using System;
using System.Runtime.Serialization;

namespace OCCPort
{
	[Serializable]
	internal class Standard_NullObject : Exception
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