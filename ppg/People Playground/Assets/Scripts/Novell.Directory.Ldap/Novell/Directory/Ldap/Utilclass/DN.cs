using System;
using System.Collections;
using System.Globalization;

namespace Novell.Directory.Ldap.Utilclass
{
	public class DN
	{
		private const int LOOK_FOR_RDN_ATTR_TYPE = 1;

		private const int ALPHA_ATTR_TYPE = 2;

		private const int OID_ATTR_TYPE = 3;

		private const int LOOK_FOR_RDN_VALUE = 4;

		private const int QUOTED_RDN_VALUE = 5;

		private const int HEX_RDN_VALUE = 6;

		private const int UNQUOTED_RDN_VALUE = 7;

		private ArrayList rdnList;

		public virtual ArrayList RDNs
		{
			get
			{
				int count = rdnList.Count;
				ArrayList arrayList = new ArrayList(count);
				for (int i = 0; i < count; i++)
				{
					arrayList.Add(rdnList[i]);
				}
				return arrayList;
			}
		}

		public virtual DN Parent
		{
			get
			{
				DN dN = new DN();
				dN.rdnList = (ArrayList)rdnList.Clone();
				if (dN.rdnList.Count >= 1)
				{
					dN.rdnList.Remove(rdnList[0]);
				}
				return dN;
			}
		}

		private void InitBlock()
		{
			rdnList = new ArrayList();
		}

		public DN()
		{
			InitBlock();
		}

		public DN(string dnString)
		{
			InitBlock();
			if (dnString.Length == 0)
			{
				return;
			}
			char[] array = new char[dnString.Length];
			int num = 0;
			string attrType = "";
			string text = "";
			string text2 = "";
			int num2 = 0;
			RDN rDN = new RDN();
			bool flag = false;
			int num3 = 0;
			int i = 0;
			int num4 = 0;
			int num5 = 1;
			for (int num6 = dnString.Length - 1; i <= num6; i++)
			{
				char c = dnString[i];
				switch (num5)
				{
				case 1:
					while (c == ' ' && i < num6)
					{
						c = dnString[++i];
					}
					if (isAlpha(c))
					{
						if (dnString.Substring(i).StartsWith("oid.") || dnString.Substring(i).StartsWith("OID."))
						{
							i += 4;
							if (i > num6)
							{
								throw new ArgumentException(dnString);
							}
							c = dnString[i];
							if (!isDigit(c))
							{
								throw new ArgumentException(dnString);
							}
							array[num3++] = c;
							num5 = 3;
						}
						else
						{
							array[num3++] = c;
							num5 = 2;
						}
					}
					else if (isDigit(c))
					{
						i--;
						num5 = 3;
					}
					else if (char.GetUnicodeCategory(c) != UnicodeCategory.SpaceSeparator)
					{
						throw new ArgumentException(dnString);
					}
					break;
				case 2:
					if (isAlpha(c) || isDigit(c) || c == '-')
					{
						array[num3++] = c;
						break;
					}
					while (c == ' ' && i < num6)
					{
						c = dnString[++i];
					}
					if (c == '=')
					{
						attrType = new string(array, 0, num3);
						num3 = 0;
						num5 = 4;
						break;
					}
					throw new ArgumentException(dnString);
				case 3:
					if (!isDigit(c))
					{
						throw new ArgumentException(dnString);
					}
					flag = ((c == '0') ? true : false);
					array[num3++] = c;
					c = dnString[++i];
					if ((isDigit(c) && flag) || (c == '.' && flag))
					{
						throw new ArgumentException(dnString);
					}
					while (isDigit(c) && i < num6)
					{
						array[num3++] = c;
						c = dnString[++i];
					}
					if (c == '.')
					{
						array[num3++] = c;
						break;
					}
					while (c == ' ' && i < num6)
					{
						c = dnString[++i];
					}
					if (c == '=')
					{
						attrType = new string(array, 0, num3);
						num3 = 0;
						num5 = 4;
						break;
					}
					throw new ArgumentException(dnString);
				case 4:
					while (true)
					{
						switch (c)
						{
						case ' ':
							if (i < num6)
							{
								goto IL_026d;
							}
							throw new ArgumentException(dnString);
						case '"':
							num5 = 5;
							num4 = i;
							break;
						case '#':
							num2 = 0;
							array[num3++] = c;
							num4 = i;
							num5 = 6;
							break;
						default:
							num4 = i;
							i--;
							num5 = 7;
							break;
						}
						break;
						IL_026d:
						c = dnString[++i];
					}
					break;
				case 7:
					switch (c)
					{
					case '\\':
						if (i >= num6)
						{
							throw new ArgumentException(dnString);
						}
						c = dnString[++i];
						if (isHexDigit(c))
						{
							if (i >= num6)
							{
								throw new ArgumentException(dnString);
							}
							char c2 = dnString[++i];
							if (!isHexDigit(c2))
							{
								throw new ArgumentException(dnString);
							}
							array[num3++] = hexToChar(c, c2);
							num = 0;
						}
						else
						{
							if (!needsEscape(c) && c != '#' && c != '=' && c != ' ')
							{
								throw new ArgumentException(dnString);
							}
							array[num3++] = c;
							num = 0;
						}
						break;
					case ' ':
						num++;
						array[num3++] = c;
						break;
					case '+':
					case ',':
					case ';':
						text = new string(array, 0, num3 - num);
						text2 = dnString.Substring(num4, i - num - num4);
						rDN.add(attrType, text, text2);
						if (c != '+')
						{
							rdnList.Add(rDN);
							rDN = new RDN();
						}
						num = 0;
						num3 = 0;
						num5 = 1;
						break;
					default:
						if (needsEscape(c))
						{
							throw new ArgumentException(dnString);
						}
						num = 0;
						array[num3++] = c;
						break;
					}
					break;
				case 5:
					switch (c)
					{
					case '"':
						text2 = dnString.Substring(num4, i + 1 - num4);
						if (i < num6)
						{
							c = dnString[++i];
						}
						while (c == ' ' && i < num6)
						{
							c = dnString[++i];
						}
						if (c == ',' || c == ';' || c == '+' || i == num6)
						{
							text = new string(array, 0, num3);
							rDN.add(attrType, text, text2);
							if (c != '+')
							{
								rdnList.Add(rDN);
								rDN = new RDN();
							}
							num = 0;
							num3 = 0;
							num5 = 1;
							break;
						}
						throw new ArgumentException(dnString);
					case '\\':
						c = dnString[++i];
						if (isHexDigit(c))
						{
							char c2 = dnString[++i];
							if (!isHexDigit(c2))
							{
								throw new ArgumentException(dnString);
							}
							array[num3++] = hexToChar(c, c2);
							num = 0;
						}
						else
						{
							if (!needsEscape(c) && c != '#' && c != '=' && c != ' ')
							{
								throw new ArgumentException(dnString);
							}
							array[num3++] = c;
							num = 0;
						}
						break;
					default:
						array[num3++] = c;
						break;
					}
					break;
				case 6:
					if (!isHexDigit(c) || i > num6)
					{
						if (num2 % 2 != 0 || num2 == 0)
						{
							throw new ArgumentException(dnString);
						}
						text2 = dnString.Substring(num4, i - num4);
						while (c == ' ' && i < num6)
						{
							c = dnString[++i];
						}
						if (c != ',' && c != ';' && c != '+' && i != num6)
						{
							throw new ArgumentException(dnString);
						}
						text = new string(array, 0, num3);
						rDN.add(attrType, text, text2);
						if (c != '+')
						{
							rdnList.Add(rDN);
							rDN = new RDN();
						}
						num3 = 0;
						num5 = 1;
					}
					else
					{
						array[num3++] = c;
						num2++;
					}
					break;
				}
			}
			if (num5 == 7 || (num5 == 6 && num2 % 2 == 0 && num2 != 0))
			{
				text = new string(array, 0, num3 - num);
				text2 = dnString.Substring(num4, i - num - num4);
				rDN.add(attrType, text, text2);
				rdnList.Add(rDN);
				return;
			}
			if (num5 == 4)
			{
				text = "";
				text2 = dnString.Substring(num4);
				rDN.add(attrType, text, text2);
				rdnList.Add(rDN);
				return;
			}
			throw new ArgumentException(dnString);
		}

		private bool isAlpha(char ch)
		{
			if ((ch < '[' && ch > '@') || (ch < '{' && ch > '`'))
			{
				return true;
			}
			return false;
		}

		private bool isDigit(char ch)
		{
			if (ch < ':' && ch > '/')
			{
				return true;
			}
			return false;
		}

		private static bool isHexDigit(char ch)
		{
			if ((ch < ':' && ch > '/') || (ch < 'G' && ch > '@') || (ch < 'g' && ch > '`'))
			{
				return true;
			}
			return false;
		}

		private bool needsEscape(char ch)
		{
			if (ch == ',' || ch == '+' || ch == '"' || ch == ';' || ch == '<' || ch == '>' || ch == '\\')
			{
				return true;
			}
			return false;
		}

		private static char hexToChar(char hex1, char hex0)
		{
			int num;
			if (hex1 < ':' && hex1 > '/')
			{
				num = (hex1 - 48) * 16;
			}
			else if (hex1 < 'G' && hex1 > '@')
			{
				num = (hex1 - 55) * 16;
			}
			else
			{
				if (hex1 >= 'g' || hex1 <= '`')
				{
					throw new ArgumentException("Not hex digit");
				}
				num = (hex1 - 87) * 16;
			}
			if (hex0 < ':' && hex0 > '/')
			{
				num += hex0 - 48;
			}
			else if (hex0 < 'G' && hex0 > '@')
			{
				num += hex0 - 55;
			}
			else
			{
				if (hex0 >= 'g' || hex0 <= '`')
				{
					throw new ArgumentException("Not hex digit");
				}
				num += hex0 - 87;
			}
			return (char)num;
		}

		public override string ToString()
		{
			int count = rdnList.Count;
			string text = "";
			if (count < 1)
			{
				return null;
			}
			text = rdnList[0].ToString();
			for (int i = 1; i < count; i++)
			{
				text = text + "," + rdnList[i].ToString();
			}
			return text;
		}

		public ArrayList getrdnList()
		{
			return rdnList;
		}

		public override bool Equals(object toDN)
		{
			return Equals((DN)toDN);
		}

		public bool Equals(DN toDN)
		{
			int count = toDN.getrdnList().Count;
			if (rdnList.Count != count)
			{
				return false;
			}
			for (int i = 0; i < count; i++)
			{
				if (!((RDN)rdnList[i]).equals((RDN)toDN.getrdnList()[i]))
				{
					return false;
				}
			}
			return true;
		}

		public virtual string[] explodeDN(bool noTypes)
		{
			int count = rdnList.Count;
			string[] array = new string[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = ((RDN)rdnList[i]).toString(noTypes);
			}
			return array;
		}

		public virtual int countRDNs()
		{
			return rdnList.Count;
		}

		public virtual bool isDescendantOf(DN containerDN)
		{
			int num = containerDN.rdnList.Count - 1;
			int num2 = rdnList.Count - 1;
			while (!((RDN)rdnList[num2]).equals((RDN)containerDN.rdnList[num]))
			{
				num2--;
				if (num2 <= 0)
				{
					return false;
				}
			}
			num--;
			num2--;
			while (num >= 0 && num2 >= 0)
			{
				if (!((RDN)rdnList[num2]).equals((RDN)containerDN.rdnList[num]))
				{
					return false;
				}
				num--;
				num2--;
			}
			if (num2 == 0 && num == 0)
			{
				return false;
			}
			return true;
		}

		public virtual void addRDN(RDN rdn)
		{
			rdnList.Insert(0, rdn);
		}

		public virtual void addRDNToFront(RDN rdn)
		{
			rdnList.Insert(0, rdn);
		}

		public virtual void addRDNToBack(RDN rdn)
		{
			rdnList.Add(rdn);
		}
	}
}
