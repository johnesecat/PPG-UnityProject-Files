using System;
using System.Collections.Generic;
using System.Linq;

namespace ImmersiveVRTools.Runtime.Common.Extensions
{
	public static class EnumExtensions
	{
		public static IEnumerable<Enum> GetFlags(this Enum value)
		{
			return GetFlags(value, Enum.GetValues(value.GetType()).Cast<Enum>().ToArray());
		}

		private static IEnumerable<Enum> GetFlags(Enum value, Enum[] values)
		{
			ulong num = Convert.ToUInt64(value);
			List<Enum> list = new List<Enum>();
			for (int num2 = values.Length - 1; num2 >= 0; num2--)
			{
				ulong num3 = Convert.ToUInt64(values[num2]);
				if (num2 == 0 && num3 == 0L)
				{
					break;
				}
				if ((num & num3) == num3)
				{
					list.Add(values[num2]);
					num -= num3;
				}
			}
			if (num != 0L)
			{
				return Enumerable.Empty<Enum>();
			}
			if (Convert.ToUInt64(value) != 0L)
			{
				return Enumerable.Reverse(list);
			}
			if (num == Convert.ToUInt64(value) && values.Length != 0 && Convert.ToUInt64(values[0]) == 0L)
			{
				return values.Take(1);
			}
			return Enumerable.Empty<Enum>();
		}

		private static IEnumerable<Enum> GetFlagValues(Type enumType)
		{
			ulong flag = 1uL;
			foreach (Enum item in Enum.GetValues(enumType).Cast<Enum>())
			{
				ulong num = Convert.ToUInt64(item);
				if (num != 0L)
				{
					while (flag < num)
					{
						flag <<= 1;
					}
					if (flag == num)
					{
						yield return item;
					}
				}
			}
		}

		public static Dictionary<TEnum, TMappedValue> AsAllElementsRequiredMap<TEnum, TMappedValue>(this Dictionary<TEnum, TMappedValue> enumMap)
		{
			return enumMap.AsAllElementsRequiredMap(new List<TEnum>());
		}

		public static Dictionary<TEnum, TMappedValue> AsAllElementsRequiredMap<TEnum, TMappedValue>(this Dictionary<TEnum, TMappedValue> enumMap, List<TEnum> ignoreValues)
		{
			Type typeFromHandle = typeof(TEnum);
			List<TEnum> enumMapValues = enumMap.Select((KeyValuePair<TEnum, TMappedValue> em) => em.Key).ToList();
			List<TEnum> source = (from TEnum ev in Enum.GetValues(typeFromHandle)
				where !enumMapValues.Contains(ev)
				select ev).ToList().Except(ignoreValues).ToList();
			if (source.Any())
			{
				throw new Exception("Invalid map setup, missing values: " + string.Join(", ", source.Select((TEnum ev) => ev).ToList()));
			}
			return enumMap;
		}
	}
}
