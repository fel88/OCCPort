using System;
using System.Runtime.Serialization;

namespace OCCPort
{
	[Serializable]
	internal class Aspect_IdentDefinitionError : Exception
	{
		public Aspect_IdentDefinitionError()
		{
		}

		public Aspect_IdentDefinitionError(string message) : base(message)
		{
		}

		public Aspect_IdentDefinitionError(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected Aspect_IdentDefinitionError(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}