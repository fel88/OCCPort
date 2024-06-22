using System;
using System.Runtime.Serialization;

namespace OCCPort
{
	[Serializable]
	internal class Standard_OutOfRange : Exception
	{
		public Standard_OutOfRange()
		{
		}

		public Standard_OutOfRange(string message) : base(message)
		{
		}

		public Standard_OutOfRange(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected Standard_OutOfRange(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}