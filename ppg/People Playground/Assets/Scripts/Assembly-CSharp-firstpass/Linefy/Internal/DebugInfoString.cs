using System;

namespace Linefy.Internal
{
	[Serializable]
	public struct DebugInfoString
	{
		[InfoString]
		public string text;

		public static implicit operator DebugInfoString(string s)
		{
			DebugInfoString result = default(DebugInfoString);
			result.text = s;
			return result;
		}
	}
}
