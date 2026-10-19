using System.Collections.Generic;

namespace Newtonsoft.Json.UnityConverters.Helpers
{
	internal static class EnumerableExtensions
	{
		public static IEnumerable<T> WhereNotNullRef<T>(this IEnumerable<T> values) where T : class
		{
			foreach (T value in values)
			{
				if (value != null)
				{
					yield return value;
				}
			}
		}

		public static IEnumerable<T> WhereNotNullValue<T>(this IEnumerable<T?> values) where T : struct
		{
			foreach (T? value in values)
			{
				if (value.HasValue)
				{
					yield return value.Value;
				}
			}
		}
	}
}
