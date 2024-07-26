using System;
using System.Runtime.Serialization;

namespace OCCPort
{
	[Serializable]
	internal class Standard_Failure : Exception
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
	}
}