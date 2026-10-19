using System;
using System.Collections.Generic;

namespace Ceras.Helpers
{
	internal static class MiscHelpers
	{
		public static string Singular<T>(this T enumValue)
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException();
			}
			string text = enumValue.ToString();
			if (text.EndsWith("ies"))
			{
				return text.Substring(0, text.Length - 3) + "y";
			}
			return text.TrimEnd('s');
		}

		public static string CleanMemberName(string name)
		{
			if (name.StartsWith("m_"))
			{
				return name.Remove(0, 2);
			}
			if (name.StartsWith("_"))
			{
				return name.Remove(0, 1);
			}
			return name;
		}

		public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (keySelector == null)
			{
				throw new ArgumentNullException("keySelector");
			}
			return _();
			IEnumerable<TSource> _()
			{
				HashSet<TKey> knownKeys = new HashSet<TKey>();
				foreach (TSource item in source)
				{
					if (knownKeys.Add(keySelector(item)))
					{
						yield return item;
					}
				}
			}
		}
	}
}
