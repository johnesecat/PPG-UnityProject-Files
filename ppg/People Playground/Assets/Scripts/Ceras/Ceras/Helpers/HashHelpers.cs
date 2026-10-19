namespace Ceras.Helpers
{
	internal static class HashHelpers
	{
		internal static readonly int[] SizeOneIntArray = new int[1];

		internal static int PowerOf2(int v)
		{
			if ((v & (v - 1)) == 0)
			{
				return v;
			}
			int num;
			for (num = 2; num < v; num <<= 1)
			{
			}
			return num;
		}
	}
}
