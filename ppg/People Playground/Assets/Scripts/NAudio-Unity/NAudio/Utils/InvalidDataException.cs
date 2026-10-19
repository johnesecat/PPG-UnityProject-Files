using System;

namespace NAudio.Utils
{
	public class InvalidDataException : global::System.Exception
	{
		public InvalidDataException()
			: base("")
		{
		}

		public InvalidDataException(string message)
			: base(message)
		{
		}

		public InvalidDataException(string message, global::System.Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
