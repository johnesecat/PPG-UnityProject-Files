using System;

namespace Ceras.Exceptions
{
	public class CerasException : Exception
	{
		public CerasException(string message)
			: base(message)
		{
		}
	}
}
