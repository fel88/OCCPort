using System;
using System.Runtime.Serialization;

namespace OCCPort
{
	[Serializable]
	internal class Standard_ConstructionError : Exception
	{
		public Standard_ConstructionError()
		{
		}

		public Standard_ConstructionError(string message) : base(message)
		{
		}

		public Standard_ConstructionError(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected Standard_ConstructionError(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}