using System;
using System.IO;
using System.Text;

namespace Novell.Directory.Ldap.Utilclass
{
	public class Base64
	{
		private static readonly char[] emap = new char[64]
		{
			'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
			'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
			'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd',
			'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n',
			'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x',
			'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7',
			'8', '9', '+', '/'
		};

		private static readonly sbyte[] dmap = new sbyte[128]
		{
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 62, 0, 0, 0, 63, 52, 53,
			54, 55, 56, 57, 58, 59, 60, 61, 0, 0,
			0, 0, 0, 0, 0, 0, 1, 2, 3, 4,
			5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
			15, 16, 17, 18, 19, 20, 21, 22, 23, 24,
			25, 0, 0, 0, 0, 0, 0, 26, 27, 28,
			29, 30, 31, 32, 33, 34, 35, 36, 37, 38,
			39, 40, 41, 42, 43, 44, 45, 46, 47, 48,
			49, 50, 51, 0, 0, 0, 0, 0
		};

		private static readonly sbyte[][] lowerBoundMask = new sbyte[6][]
		{
			new sbyte[2],
			new sbyte[2] { 30, 0 },
			new sbyte[2] { 15, 32 },
			new sbyte[2] { 7, 48 },
			new sbyte[2] { 2, 56 },
			new sbyte[2] { 1, 60 }
		};

		private static sbyte continuationMask = (sbyte)SupportClass.Identity(192L);

		private static sbyte continuationResult = (sbyte)SupportClass.Identity(128L);

		private Base64()
		{
		}

		public static string encode(string inputString)
		{
			try
			{
				return encode(SupportClass.ToSByteArray(Encoding.GetEncoding("utf-8").GetBytes(inputString)));
			}
			catch (IOException)
			{
				throw new SystemException("US-ASCII String encoding not supported by JVM");
			}
		}

		[CLSCompliant(false)]
		public static string encode(sbyte[] inputBytes)
		{
			bool flag = false;
			bool flag2 = false;
			int num = inputBytes.Length;
			if (num == 0)
			{
				return new StringBuilder("").ToString();
			}
			int num2 = ((num % 3 != 0) ? (num / 3 + 1) : (num / 3));
			if (num % 3 == 1)
			{
				flag2 = true;
			}
			else if (num % 3 == 2)
			{
				flag = true;
			}
			char[] array = new char[num2 * 4];
			int num3 = 0;
			int num4 = 0;
			int num5 = 1;
			while (num3 < num)
			{
				int num6 = 0xFF & inputBytes[num3];
				array[num4] = emap[num6 >> 2];
				if (num5 == num2 && flag2)
				{
					array[num4 + 1] = emap[(num6 & 3) << 4];
					array[num4 + 2] = '=';
					array[num4 + 3] = '=';
					break;
				}
				int num7 = 0xFF & inputBytes[num3 + 1];
				array[num4 + 1] = emap[((num6 & 3) << 4) + ((num7 & 0xF0) >> 4)];
				if (num5 == num2 && flag)
				{
					array[num4 + 2] = emap[(num7 & 0xF) << 2];
					array[num4 + 3] = '=';
					break;
				}
				int num8 = 0xFF & inputBytes[num3 + 2];
				array[num4 + 2] = emap[((num7 & 0xF) << 2) | ((num8 & 0xC0) >> 6)];
				array[num4 + 3] = emap[num8 & 0x3F];
				num3 += 3;
				num4 += 4;
				num5++;
			}
			return new string(array);
		}

		[CLSCompliant(false)]
		public static sbyte[] decode(string encodedString)
		{
			char[] destinationArray = new char[encodedString.Length];
			SupportClass.GetCharsFromString(encodedString, 0, encodedString.Length, ref destinationArray, 0);
			return decode(destinationArray);
		}

		[CLSCompliant(false)]
		public static sbyte[] decode(char[] encodedChars)
		{
			int num = encodedChars.Length;
			int num2 = num / 4;
			bool flag = false;
			bool flag2 = false;
			if (encodedChars.Length == 0)
			{
				return new sbyte[0];
			}
			if (num % 4 != 0)
			{
				throw new SystemException("Novell.Directory.Ldap.ldif_dsml.Base64Decoder: decode: mal-formatted encode value");
			}
			sbyte[] array;
			if (encodedChars[num - 1] == '=' && encodedChars[num - 2] == '=')
			{
				flag2 = true;
				array = new sbyte[num2 * 3 - 2];
			}
			else if (encodedChars[num - 1] == '=')
			{
				flag = true;
				array = new sbyte[num2 * 3 - 1];
			}
			else
			{
				array = new sbyte[num2 * 3];
			}
			int num3 = 0;
			int num4 = 0;
			int num5 = 1;
			while (num3 < num)
			{
				array[num4] = (sbyte)((dmap[(uint)encodedChars[num3]] << 2) | ((dmap[(uint)encodedChars[num3 + 1]] & 0x30) >> 4));
				if (num5 == num2 && flag2)
				{
					break;
				}
				array[num4 + 1] = (sbyte)(((dmap[(uint)encodedChars[num3 + 1]] & 0xF) << 4) | ((dmap[(uint)encodedChars[num3 + 2]] & 0x3C) >> 2));
				if (num5 == num2 && flag)
				{
					break;
				}
				array[num4 + 2] = (sbyte)(((dmap[(uint)encodedChars[num3 + 2]] & 3) << 6) | (dmap[(uint)encodedChars[num3 + 3]] & 0x3F));
				num3 += 4;
				num4 += 3;
				num5++;
			}
			return array;
		}

		[CLSCompliant(false)]
		public static sbyte[] decode(StringBuilder encodedSBuf, int start, int end)
		{
			int num = end - start;
			int num2 = num / 4;
			bool flag = false;
			bool flag2 = false;
			if (encodedSBuf.Length == 0)
			{
				return new sbyte[0];
			}
			if (num % 4 != 0)
			{
				throw new SystemException("Novell.Directory.Ldap.ldif_dsml.Base64Decoder: decode error: mal-formatted encode value");
			}
			sbyte[] array;
			if (encodedSBuf[end - 1] == '=' && encodedSBuf[end - 2] == '=')
			{
				flag2 = true;
				array = new sbyte[num2 * 3 - 2];
			}
			else if (encodedSBuf[end - 1] == '=')
			{
				flag = true;
				array = new sbyte[num2 * 3 - 1];
			}
			else
			{
				array = new sbyte[num2 * 3];
			}
			int num3 = 0;
			int num4 = 0;
			int num5 = 1;
			while (num3 < num)
			{
				array[num4] = (sbyte)((dmap[(uint)encodedSBuf[start + num3]] << 2) | ((dmap[(uint)encodedSBuf[start + num3 + 1]] & 0x30) >> 4));
				if (num5 == num2 && flag2)
				{
					break;
				}
				array[num4 + 1] = (sbyte)(((dmap[(uint)encodedSBuf[start + num3 + 1]] & 0xF) << 4) | ((dmap[(uint)encodedSBuf[start + num3 + 2]] & 0x3C) >> 2));
				if (num5 == num2 && flag)
				{
					break;
				}
				array[num4 + 2] = (sbyte)(((dmap[(uint)encodedSBuf[start + num3 + 2]] & 3) << 6) | (dmap[(uint)encodedSBuf[start + num3 + 3]] & 0x3F));
				num3 += 4;
				num4 += 3;
				num5++;
			}
			return array;
		}

		[CLSCompliant(false)]
		public static bool isLDIFSafe(sbyte[] bytes)
		{
			int num = bytes.Length;
			if (num > 0)
			{
				int num2 = bytes[0];
				if (num2 == 0 || num2 == 10 || num2 == 13 || num2 == 32 || num2 == 58 || num2 == 60 || num2 < 0)
				{
					return false;
				}
				if (bytes[num - 1] == 32)
				{
					return false;
				}
				if (num > 1)
				{
					for (int i = 1; i < bytes.Length; i++)
					{
						num2 = bytes[i];
						if (num2 == 0 || num2 == 10 || num2 == 13 || num2 < 0)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		public static bool isLDIFSafe(string str)
		{
			try
			{
				return isLDIFSafe(SupportClass.ToSByteArray(Encoding.GetEncoding("utf-8").GetBytes(str)));
			}
			catch (IOException)
			{
				throw new SystemException("UTF-8 String encoding not supported by JVM");
			}
		}

		private static int getByteCount(sbyte b)
		{
			if (b > 0)
			{
				return 0;
			}
			if ((b & 0xE0) == 192)
			{
				return 1;
			}
			if ((b & 0xF0) == 224)
			{
				return 2;
			}
			if ((b & 0xF8) == 240)
			{
				return 3;
			}
			if ((b & 0xFC) == 248)
			{
				return 4;
			}
			if ((b & 0xFF) == 252)
			{
				return 5;
			}
			return -1;
		}

		[CLSCompliant(false)]
		public static bool isValidUTF8(sbyte[] array, bool isUCS2Only)
		{
			int num = 0;
			while (num < array.Length)
			{
				int byteCount = getByteCount(array[num]);
				switch (byteCount)
				{
				case 0:
					num++;
					continue;
				default:
					if (num + byteCount < array.Length && (!isUCS2Only || byteCount < 3))
					{
						break;
					}
					goto case -1;
				case -1:
					return false;
				}
				if ((lowerBoundMask[byteCount][0] & array[num]) == 0 && (lowerBoundMask[byteCount][1] & array[num + 1]) == 0)
				{
					return false;
				}
				for (int i = 1; i <= byteCount; i++)
				{
					if ((array[num + i] & continuationMask) != continuationResult)
					{
						return false;
					}
				}
				num += byteCount + 1;
			}
			return true;
		}
	}
}
