using System;
using System.Runtime.Serialization;

namespace OCCPort
{
	[Serializable]
	internal class Standard_NotImplemented : Exception
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

		protected Standard_NotImplemented(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}