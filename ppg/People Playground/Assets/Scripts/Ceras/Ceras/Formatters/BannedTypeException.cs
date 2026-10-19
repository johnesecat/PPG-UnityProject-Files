using System;

namespace Ceras.Formatters
{
	internal class BannedTypeException : Exception
	{
		public BannedTypeException(string message)
			: base(message)
		{
		}
	}
}
