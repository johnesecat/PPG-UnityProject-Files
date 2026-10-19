using System;
using System.Text;

namespace NAudio.Utils
{
	public static class ByteArrayExtensions
	{
		public static bool IsEntirelyNull(byte[] buffer)
		{
			for (int i = 0; i < buffer.Length; i++)
			{
				if (buffer[i] != 0)
				{
					return false;
				}
			}
			return true;
		}

		public static string DescribeAsHex(byte[] buffer, string separator, int bytesPerLine)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			StringBuilder val = new StringBuilder();
			int num = 0;
			foreach (byte b in buffer)
			{
				val.AppendFormat("{0:X2}{1}", (object)b, (object)separator);
				if (++num % bytesPerLine == 0)
				{
					val.Append("\r\n");
				}
			}
			val.Append("\r\n");
			return ((object)val).ToString();
		}

		public static string DecodeAsString(byte[] buffer, int offset, int length, Encoding encoding)
		{
			for (int i = 0; i < length; i++)
			{
				if (buffer[offset + i] == 0)
				{
					length = i;
				}
			}
			return encoding.GetString(buffer, offset, length);
		}

		public static byte[] Concat(params byte[][] byteArrays)
		{
			int num = 0;
			byte[][] array = byteArrays;
			foreach (byte[] array2 in array)
			{
				num += array2.Length;
			}
			if (num <= 0)
			{
				return new byte[0];
			}
			byte[] array3 = new byte[num];
			int num2 = 0;
			array = byteArrays;
			foreach (byte[] array4 in array)
			{
				global::System.Array.Copy((global::System.Array)array4, 0, (global::System.Array)array3, num2, array4.Length);
				num2 += array4.Length;
			}
			return array3;
		}
	}
}
