using System;
using Ceras.Helpers;

namespace Ceras.Formatters
{
	internal static class FormatterHelper
	{
		public static bool IsFormatterMatch(IFormatter formatter, Type type)
		{
			Type type2 = ReflectionHelper.FindClosedType(formatter.GetType(), typeof(IFormatter<>)).GetGenericArguments()[0];
			return type == type2;
		}

		public static void ThrowOnMismatch(IFormatter formatter, Type typeToFormat)
		{
			if (!IsFormatterMatch(formatter, typeToFormat))
			{
				throw new InvalidOperationException("The given formatter '" + formatter.GetType().FullName + "' is not an exact match for the formatted type '" + typeToFormat.FullName + "'");
			}
		}
	}
}
