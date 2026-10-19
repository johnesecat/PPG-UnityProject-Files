using System;
using System.IO;

namespace Novell.Directory.Ldap.Utilclass
{
	public class SchemaTokenCreator
	{
		private string basestring;

		private bool cppcomments;

		private bool ccomments;

		private bool iseolsig;

		private bool cidtolower;

		private bool pushedback;

		private int peekchar;

		private sbyte[] ctype;

		private int linenumber = 1;

		private int ichar = 1;

		private char[] buf;

		private StreamReader reader;

		private StringReader sreader;

		private Stream input;

		public string StringValue;

		public double NumberValue;

		public int lastttype;

		public int CurrentLine => linenumber;

		private void Initialise()
		{
			ctype = new sbyte[256];
			buf = new char[20];
			peekchar = int.MaxValue;
			WordCharacters(97, 122);
			WordCharacters(65, 90);
			WordCharacters(160, 255);
			WhitespaceCharacters(0, 32);
			CommentCharacter(47);
			QuoteCharacter(34);
			QuoteCharacter(39);
			parseNumbers();
		}

		public SchemaTokenCreator(Stream instream)
		{
			Initialise();
			if (instream == null)
			{
				throw new NullReferenceException();
			}
			input = instream;
		}

		public SchemaTokenCreator(StreamReader r)
		{
			Initialise();
			if (r == null)
			{
				throw new NullReferenceException();
			}
			reader = r;
		}

		public SchemaTokenCreator(StringReader r)
		{
			Initialise();
			if (r == null)
			{
				throw new NullReferenceException();
			}
			sreader = r;
		}

		public void pushBack()
		{
			pushedback = true;
		}

		public string ToStringValue()
		{
			switch (lastttype)
			{
			case -1:
				return "EOF";
			case 10:
				return "EOL";
			case -3:
				return StringValue;
			case -5:
				return StringValue;
			case -4:
			case -2:
				return "n=" + NumberValue;
			default:
			{
				if (lastttype < 256 && ((uint)ctype[lastttype] & 8u) != 0)
				{
					return StringValue;
				}
				char[] array = new char[3];
				array[0] = (array[2] = '\'');
				array[1] = (char)lastttype;
				return new string(array);
			}
			}
		}

		public void WordCharacters(int min, int max)
		{
			if (min < 0)
			{
				min = 0;
			}
			if (max >= ctype.Length)
			{
				max = ctype.Length - 1;
			}
			while (min <= max)
			{
				ctype[min++] |= 4;
			}
		}

		public void WhitespaceCharacters(int min, int max)
		{
			if (min < 0)
			{
				min = 0;
			}
			if (max >= ctype.Length)
			{
				max = ctype.Length - 1;
			}
			while (min <= max)
			{
				ctype[min++] = 1;
			}
		}

		public void OrdinaryCharacters(int min, int max)
		{
			if (min < 0)
			{
				min = 0;
			}
			if (max >= ctype.Length)
			{
				max = ctype.Length - 1;
			}
			while (min <= max)
			{
				ctype[min++] = 0;
			}
		}

		public void OrdinaryCharacter(int ch)
		{
			if (ch >= 0 && ch < ctype.Length)
			{
				ctype[ch] = 0;
			}
		}

		public void CommentCharacter(int ch)
		{
			if (ch >= 0 && ch < ctype.Length)
			{
				ctype[ch] = 16;
			}
		}

		public void InitTable()
		{
			int num = ctype.Length;
			while (--num >= 0)
			{
				ctype[num] = 0;
			}
		}

		public void QuoteCharacter(int ch)
		{
			if (ch >= 0 && ch < ctype.Length)
			{
				ctype[ch] = 8;
			}
		}

		public void parseNumbers()
		{
			for (int i = 48; i <= 57; i++)
			{
				ctype[i] |= 2;
			}
			ctype[46] |= 2;
			ctype[45] |= 2;
		}

		private int read()
		{
			if (sreader != null)
			{
				return sreader.Read();
			}
			if (reader != null)
			{
				return reader.Read();
			}
			if (input != null)
			{
				return input.ReadByte();
			}
			throw new SystemException();
		}

		public int nextToken()
		{
			if (pushedback)
			{
				pushedback = false;
				return lastttype;
			}
			StringValue = null;
			int num = peekchar;
			if (num < 0)
			{
				num = int.MaxValue;
			}
			if (num == 2147483646)
			{
				num = read();
				if (num < 0)
				{
					return lastttype = -1;
				}
				if (num == 10)
				{
					num = int.MaxValue;
				}
			}
			if (num == int.MaxValue)
			{
				num = read();
				if (num < 0)
				{
					return lastttype = -1;
				}
			}
			lastttype = num;
			peekchar = int.MaxValue;
			int num2 = ((num < 256) ? ctype[num] : 4);
			while (((uint)num2 & (true ? 1u : 0u)) != 0)
			{
				if (num == 13)
				{
					linenumber++;
					if (iseolsig)
					{
						peekchar = 2147483646;
						return lastttype = 10;
					}
					num = read();
					if (num == 10)
					{
						num = read();
					}
				}
				else
				{
					if (num == 10)
					{
						linenumber++;
						if (iseolsig)
						{
							return lastttype = 10;
						}
					}
					num = read();
				}
				if (num < 0)
				{
					return lastttype = -1;
				}
				num2 = ((num < 256) ? ctype[num] : 4);
			}
			if (((uint)num2 & 2u) != 0)
			{
				bool flag = false;
				if (num == 45)
				{
					num = read();
					switch (num)
					{
					default:
						peekchar = num;
						return lastttype = 45;
					case 46:
					case 48:
					case 49:
					case 50:
					case 51:
					case 52:
					case 53:
					case 54:
					case 55:
					case 56:
					case 57:
						break;
					}
					flag = true;
				}
				double num3 = 0.0;
				int num4 = 0;
				int num5 = 0;
				while (true)
				{
					if (num == 46 && num5 == 0)
					{
						num5 = 1;
					}
					else
					{
						if (48 > num || num > 57)
						{
							break;
						}
						num3 = num3 * 10.0 + (double)(num - 48);
						num4 += num5;
					}
					num = read();
				}
				peekchar = num;
				if (num4 != 0)
				{
					double num6 = 10.0;
					for (num4--; num4 > 0; num4--)
					{
						num6 *= 10.0;
					}
					num3 /= num6;
				}
				NumberValue = (flag ? (0.0 - num3) : num3);
				return lastttype = -2;
			}
			if (((uint)num2 & 4u) != 0)
			{
				int num7 = 0;
				do
				{
					if (num7 >= buf.Length)
					{
						char[] destinationArray = new char[buf.Length * 2];
						Array.Copy(buf, 0, destinationArray, 0, buf.Length);
						buf = destinationArray;
					}
					buf[num7++] = (char)num;
					num = read();
					num2 = ((num < 0) ? 1 : ((num < 256) ? ctype[num] : 4));
				}
				while (((uint)num2 & 6u) != 0);
				peekchar = num;
				StringValue = new string(buf, 0, num7);
				if (cidtolower)
				{
					StringValue = StringValue.ToLower();
				}
				return lastttype = -3;
			}
			if (((uint)num2 & 8u) != 0)
			{
				lastttype = num;
				int num8 = 0;
				int num9 = read();
				while (num9 >= 0 && num9 != lastttype && num9 != 10 && num9 != 13)
				{
					if (num9 == 92)
					{
						num = read();
						int num10 = num;
						if (num >= 48 && num <= 55)
						{
							num -= 48;
							int num11 = read();
							if (48 <= num11 && num11 <= 55)
							{
								num = (num << 3) + (num11 - 48);
								num11 = read();
								if (48 <= num11 && num11 <= 55 && num10 <= 51)
								{
									num = (num << 3) + (num11 - 48);
									num9 = read();
								}
								else
								{
									num9 = num11;
								}
							}
							else
							{
								num9 = num11;
							}
						}
						else
						{
							switch (num)
							{
							case 102:
								num = 12;
								break;
							case 97:
								num = 7;
								break;
							case 98:
								num = 8;
								break;
							case 118:
								num = 11;
								break;
							case 110:
								num = 10;
								break;
							case 114:
								num = 13;
								break;
							case 116:
								num = 9;
								break;
							}
							num9 = read();
						}
					}
					else
					{
						num = num9;
						num9 = read();
					}
					if (num8 >= buf.Length)
					{
						char[] destinationArray2 = new char[buf.Length * 2];
						Array.Copy(buf, 0, destinationArray2, 0, buf.Length);
						buf = destinationArray2;
					}
					buf[num8++] = (char)num;
				}
				peekchar = ((num9 == lastttype) ? int.MaxValue : num9);
				StringValue = new string(buf, 0, num8);
				return lastttype;
			}
			if (num == 47 && (cppcomments || ccomments))
			{
				num = read();
				if (num == 42 && ccomments)
				{
					int num12 = 0;
					while ((num = read()) != 47 || num12 != 42)
					{
						switch (num)
						{
						case 13:
							linenumber++;
							num = read();
							if (num == 10)
							{
								num = read();
							}
							break;
						case 10:
							linenumber++;
							num = read();
							break;
						}
						if (num < 0)
						{
							return lastttype = -1;
						}
						num12 = num;
					}
					return nextToken();
				}
				if (num == 47 && cppcomments)
				{
					while ((num = read()) != 10 && num != 13 && num >= 0)
					{
					}
					peekchar = num;
					return nextToken();
				}
				if (((uint)ctype[47] & 0x10u) != 0)
				{
					while ((num = read()) != 10 && num != 13 && num >= 0)
					{
					}
					peekchar = num;
					return nextToken();
				}
				peekchar = num;
				return lastttype = 47;
			}
			if (((uint)num2 & 0x10u) != 0)
			{
				while ((num = read()) != 10 && num != 13 && num >= 0)
				{
				}
				peekchar = num;
				return nextToken();
			}
			return lastttype = num;
		}
	}
}
