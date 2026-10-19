using System;
using Ceras.Formatters;
using Ceras.Helpers;

namespace Ceras
{
	internal class PreviousFormatter : PreviousNameAttribute
	{
		public Type FormatterType { get; }

		public PreviousFormatter(Type formatterType)
			: base(null)
		{
			CheckType(formatterType);
			FormatterType = formatterType;
		}

		public PreviousFormatter(string previousName, Type formatterType)
			: base(previousName)
		{
			CheckType(formatterType);
			FormatterType = formatterType;
		}

		private static void CheckType(Type formatterType)
		{
			if (!typeof(IFormatter).IsAssignableFrom(formatterType))
			{
				throw new Exception("The provided type " + formatterType.FriendlyName() + " is not valid for 'PreviousFormatter', it needs to be a type that implements IFormatter<T>");
			}
		}
	}
}
