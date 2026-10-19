using System;
using System.Collections.Generic;
using System.Reflection;
using Ceras.Helpers;

namespace Ceras
{
	internal class SchemaMemberComparer : IComparer<SchemaMember>
	{
		public static readonly SchemaMemberComparer Instance = new SchemaMemberComparer();

		public int Compare(SchemaMember x, SchemaMember y)
		{
			string comparisonName = GetComparisonName(x);
			string comparisonName2 = GetComparisonName(y);
			return string.Compare(comparisonName, comparisonName2, StringComparison.Ordinal);
		}

		private static string GetComparisonName(SchemaMember m)
		{
			return (IsFixedSize(m.MemberType) ? "" : "") + m.MemberType.FullName + m.PersistentName + m.MemberInfo.DeclaringType.FullName + ((m.MemberInfo is FieldInfo) ? "f" : "p");
		}

		private static bool IsFixedSize(Type t)
		{
			if (t.IsPrimitive)
			{
				return true;
			}
			if (!t.IsValueType)
			{
				return false;
			}
			FieldInfo[] fields = t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < fields.Length; i++)
			{
				if (!IsFixedSize(fields[i].FieldType))
				{
					return false;
				}
			}
			return true;
		}
	}
}
