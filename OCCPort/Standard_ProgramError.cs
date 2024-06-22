using System;
using System.Runtime.Serialization;

namespace OCCPort
{
	[Serializable]
	public  class Standard_ProgramError : Exception
	{
		public Standard_ProgramError()
		{
		}

		public Standard_ProgramError(string message) : base(message)
		{
		}

		public Standard_ProgramError(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected Standard_ProgramError(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}