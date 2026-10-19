using System;
using System.Text;

namespace NAudio.Utils
{
	public class ChunkIdentifier
	{
		public static int ChunkIdentifierToInt32(string s)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			if (s.get_Length() != 4)
			{
				throw new ArgumentException("Must be a four character string");
			}
			byte[] bytes = Encoding.get_UTF8().GetBytes(s);
			if (bytes.Length != 4)
			{
				throw new ArgumentException("Must encode to exactly four bytes");
			}
			return BitConverter.ToInt32(bytes, 0);
		}
	}
}
