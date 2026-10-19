using System.Collections.Generic;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
	public class ListHashGenerator
	{
		public static int GetHashBasedOnElements<T>(IEnumerable<T> items) where T : struct
		{
			int num = 19;
			foreach (T item in items)
			{
				num = num * 31 + item.GetHashCode();
			}
			return num;
		}
	}
}
