namespace ImmersiveVRTools.Runtime.Common.Extensions
{
	public static class StringExtensions
	{
		public static string TrimExcess(this string str, int maxLenght, string postfixToIfTrimmed = "...")
		{
			if (!str.TryTrimExcess(maxLenght, out var trimmedValue, postfixToIfTrimmed))
			{
				return str;
			}
			return trimmedValue;
		}

		public static bool TryTrimExcess(this string str, int maxLenght, out string trimmedValue, string postfixToIfTrimmed = "...")
		{
			bool flag = str.Length > maxLenght;
			trimmedValue = (flag ? (str.Substring(0, maxLenght - postfixToIfTrimmed.Length) + postfixToIfTrimmed) : str);
			return flag;
		}
	}
}
