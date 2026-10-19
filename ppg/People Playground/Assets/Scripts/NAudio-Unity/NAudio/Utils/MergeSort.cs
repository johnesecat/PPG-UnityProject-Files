using System;
using System.Collections.Generic;

namespace NAudio.Utils
{
	internal class MergeSort
	{
		private static void Sort<T>(global::System.Collections.Generic.IList<T> list, int lowIndex, int highIndex, IComparer<T> comparer)
		{
			if (lowIndex >= highIndex)
			{
				return;
			}
			int num = (lowIndex + highIndex) / 2;
			Sort(list, lowIndex, num, comparer);
			Sort(list, num + 1, highIndex, comparer);
			int num2 = num;
			int num3 = num + 1;
			while (lowIndex <= num2 && num3 <= highIndex)
			{
				if (comparer.Compare(list.get_Item(lowIndex), list.get_Item(num3)) <= 0)
				{
					lowIndex++;
					continue;
				}
				T val = list.get_Item(num3);
				for (int num4 = num3 - 1; num4 >= lowIndex; num4--)
				{
					list.set_Item(num4 + 1, list.get_Item(num4));
				}
				list.set_Item(lowIndex, val);
				lowIndex++;
				num2++;
				num3++;
			}
		}

		public static void Sort<T>(global::System.Collections.Generic.IList<T> list) where T : IComparable<T>
		{
			Sort(list, 0, ((global::System.Collections.Generic.ICollection<T>)list).get_Count() - 1, (IComparer<T>)(object)Comparer<T>.get_Default());
		}

		public static void Sort<T>(global::System.Collections.Generic.IList<T> list, IComparer<T> comparer)
		{
			Sort(list, 0, ((global::System.Collections.Generic.ICollection<T>)list).get_Count() - 1, comparer);
		}
	}
}
