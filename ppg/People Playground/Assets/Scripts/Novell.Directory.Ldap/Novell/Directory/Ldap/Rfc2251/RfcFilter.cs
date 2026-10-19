using System;
using System.Collections;
using System.IO;
using System.Text;
using Novell.Directory.Ldap.Asn1;
using Novell.Directory.Ldap.Utilclass;

namespace Novell.Directory.Ldap.Rfc2251
{
	public class RfcFilter : Asn1Choice
	{
		private class FilterIterator : IEnumerator
		{
			private RfcFilter enclosingInstance;

			internal Asn1Tagged root;

			internal bool tagReturned;

			internal int index = -1;

			private bool hasMore = true;

			public virtual object Current
			{
				get
				{
					object result = null;
					if (!tagReturned)
					{
						tagReturned = true;
						result = root.getIdentifier().Tag;
					}
					else
					{
						Asn1Object asn1Object = root.taggedValue();
						if (asn1Object is RfcLdapString)
						{
							hasMore = false;
							result = ((RfcLdapString)asn1Object).stringValue();
						}
						else if (asn1Object is RfcSubstringFilter)
						{
							RfcSubstringFilter rfcSubstringFilter = (RfcSubstringFilter)asn1Object;
							if (index == -1)
							{
								index = 0;
								result = ((RfcAttributeDescription)rfcSubstringFilter.get_Renamed(0)).stringValue();
							}
							else if (index % 2 == 0)
							{
								result = ((Asn1Tagged)((Asn1SequenceOf)rfcSubstringFilter.get_Renamed(1)).get_Renamed(index / 2)).getIdentifier().Tag;
								index++;
							}
							else
							{
								result = ((RfcLdapString)((Asn1Tagged)((Asn1SequenceOf)rfcSubstringFilter.get_Renamed(1)).get_Renamed(index / 2)).taggedValue()).stringValue();
								index++;
							}
							if (index / 2 >= ((Asn1SequenceOf)rfcSubstringFilter.get_Renamed(1)).size())
							{
								hasMore = false;
							}
						}
						else if (asn1Object is RfcAttributeValueAssertion)
						{
							RfcAttributeValueAssertion rfcAttributeValueAssertion = (RfcAttributeValueAssertion)asn1Object;
							if (index == -1)
							{
								result = rfcAttributeValueAssertion.AttributeDescription;
								index = 1;
							}
							else if (index == 1)
							{
								result = rfcAttributeValueAssertion.AssertionValue;
								index = 2;
								hasMore = false;
							}
						}
						else if (asn1Object is RfcMatchingRuleAssertion)
						{
							RfcMatchingRuleAssertion obj = (RfcMatchingRuleAssertion)asn1Object;
							if (index == -1)
							{
								index = 0;
							}
							result = ((Asn1OctetString)((Asn1Tagged)obj.get_Renamed(index++)).taggedValue()).stringValue();
							if (index > 2)
							{
								hasMore = false;
							}
						}
						else if (asn1Object is Asn1SetOf)
						{
							Asn1SetOf asn1SetOf = (Asn1SetOf)asn1Object;
							if (index == -1)
							{
								index = 0;
							}
							result = new FilterIterator(enclosingInstance, (Asn1Tagged)asn1SetOf.get_Renamed(index++));
							if (index >= asn1SetOf.size())
							{
								hasMore = false;
							}
						}
						else if (asn1Object is Asn1Tagged)
						{
							result = new FilterIterator(enclosingInstance, (Asn1Tagged)asn1Object);
							hasMore = false;
						}
					}
					return result;
				}
			}

			public RfcFilter Enclosing_Instance => enclosingInstance;

			public void Reset()
			{
			}

			private void InitBlock(RfcFilter enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}

			public FilterIterator(RfcFilter enclosingInstance, Asn1Tagged root)
			{
				InitBlock(enclosingInstance);
				this.root = root;
			}

			public virtual bool MoveNext()
			{
				return hasMore;
			}

			public void remove()
			{
				throw new NotSupportedException("Remove is not supported on a filter iterator");
			}
		}

		internal class FilterTokenizer
		{
			private RfcFilter enclosingInstance;

			private string filter;

			private string attr;

			private int offset;

			private int filterLength;

			public virtual int OpOrAttr
			{
				get
				{
					if (offset >= filterLength)
					{
						throw new LdapLocalException("UNEXPECTED_END", 87);
					}
					switch (filter[offset])
					{
					case '&':
						offset++;
						return 0;
					case '|':
						offset++;
						return 1;
					case '!':
						offset++;
						return 2;
					default:
					{
						if (filter.Substring(offset).StartsWith(":="))
						{
							throw new LdapLocalException("NO_MATCHING_RULE", 87);
						}
						if (filter.Substring(offset).StartsWith("::=") || filter.Substring(offset).StartsWith(":::="))
						{
							throw new LdapLocalException("NO_DN_NOR_MATCHING_RULE", 87);
						}
						string text = "=~<>()";
						StringBuilder stringBuilder = new StringBuilder();
						while (text.IndexOf(filter[offset]) == -1 && !filter.Substring(offset).StartsWith(":="))
						{
							stringBuilder.Append(filter[offset++]);
						}
						attr = stringBuilder.ToString().Trim();
						if (attr.Length == 0 || attr[0] == ';')
						{
							throw new LdapLocalException("NO_ATTRIBUTE_NAME", 87);
						}
						int i;
						for (i = 0; i < attr.Length; i++)
						{
							char c = attr[i];
							if (!char.IsLetterOrDigit(c))
							{
								switch (c)
								{
								case '\\':
									throw new LdapLocalException("INVALID_ESC_IN_DESCR", 87);
								case '-':
								case '.':
								case ':':
								case ';':
									continue;
								}
								throw new LdapLocalException("INVALID_CHAR_IN_DESCR", new object[1] { c }, 87);
							}
						}
						i = attr.IndexOf(';');
						if (i != -1 && i == attr.Length - 1)
						{
							throw new LdapLocalException("NO_OPTION", 87);
						}
						return -1;
					}
					}
				}
			}

			public virtual int FilterType
			{
				get
				{
					if (offset >= filterLength)
					{
						throw new LdapLocalException("UNEXPECTED_END", 87);
					}
					if (filter.Substring(offset).StartsWith(">="))
					{
						offset += 2;
						return 5;
					}
					if (filter.Substring(offset).StartsWith("<="))
					{
						offset += 2;
						return 6;
					}
					if (filter.Substring(offset).StartsWith("~="))
					{
						offset += 2;
						return 8;
					}
					if (filter.Substring(offset).StartsWith(":="))
					{
						offset += 2;
						return 9;
					}
					if (filter[offset] == '=')
					{
						offset++;
						return 3;
					}
					throw new LdapLocalException("INVALID_FILTER_COMPARISON", 87);
				}
			}

			public virtual string Value
			{
				get
				{
					if (offset >= filterLength)
					{
						throw new LdapLocalException("UNEXPECTED_END", 87);
					}
					int num = filter.IndexOf(')', offset);
					if (num == -1)
					{
						num = filterLength;
					}
					string result = filter.Substring(offset, num - offset);
					offset = num;
					return result;
				}
			}

			public virtual string Attr => attr;

			public RfcFilter Enclosing_Instance => enclosingInstance;

			private void InitBlock(RfcFilter enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}

			public FilterTokenizer(RfcFilter enclosingInstance, string filter)
			{
				InitBlock(enclosingInstance);
				this.filter = filter;
				offset = 0;
				filterLength = filter.Length;
			}

			public void getLeftParen()
			{
				if (offset >= filterLength)
				{
					throw new LdapLocalException("UNEXPECTED_END", 87);
				}
				if (filter[offset++] != '(')
				{
					throw new LdapLocalException("EXPECTING_LEFT_PAREN", new object[1] { filter[--offset] }, 87);
				}
			}

			public void getRightParen()
			{
				if (offset >= filterLength)
				{
					throw new LdapLocalException("UNEXPECTED_END", 87);
				}
				if (filter[offset++] != ')')
				{
					throw new LdapLocalException("EXPECTING_RIGHT_PAREN", new object[1] { filter[offset - 1] }, 87);
				}
			}

			public char peekChar()
			{
				if (offset >= filterLength)
				{
					throw new LdapLocalException("UNEXPECTED_END", 87);
				}
				return filter[offset];
			}
		}

		public const int AND = 0;

		public const int OR = 1;

		public const int NOT = 2;

		public const int EQUALITY_MATCH = 3;

		public const int SUBSTRINGS = 4;

		public const int GREATER_OR_EQUAL = 5;

		public const int LESS_OR_EQUAL = 6;

		public const int PRESENT = 7;

		public const int APPROX_MATCH = 8;

		public const int EXTENSIBLE_MATCH = 9;

		public const int INITIAL = 0;

		public const int ANY = 1;

		public const int FINAL = 2;

		private FilterTokenizer ft;

		private Stack filterStack;

		private bool finalFound;

		public RfcFilter(string filter)
			: base(null)
		{
			ChoiceValue = parse(filter);
		}

		public RfcFilter()
			: base(null)
		{
			filterStack = new Stack();
		}

		private Asn1Tagged parse(string filterExpr)
		{
			if (filterExpr == null || filterExpr.Equals(""))
			{
				filterExpr = new StringBuilder("(objectclass=*)").ToString();
			}
			int num;
			if ((num = filterExpr.IndexOf('\\')) != -1)
			{
				StringBuilder stringBuilder = new StringBuilder(filterExpr);
				int num2 = num;
				while (num2 < stringBuilder.Length - 1)
				{
					char c = stringBuilder[num2++];
					if (c == '\\')
					{
						c = stringBuilder[num2];
						if (c == '*' || c == '(' || c == ')' || c == '\\')
						{
							stringBuilder.Remove(num2, num2 + 1 - num2);
							stringBuilder.Insert(num2, Convert.ToString(c, 16));
							num2 += 2;
						}
					}
				}
				filterExpr = stringBuilder.ToString();
			}
			if (filterExpr[0] != '(' && filterExpr[filterExpr.Length - 1] != ')')
			{
				filterExpr = "(" + filterExpr + ")";
			}
			char num3 = filterExpr[0];
			int length = filterExpr.Length;
			if (num3 != '(')
			{
				throw new LdapLocalException("MISSING_LEFT_PAREN", 87);
			}
			if (filterExpr[length - 1] != ')')
			{
				throw new LdapLocalException("MISSING_RIGHT_PAREN", 87);
			}
			int num4 = 0;
			for (int i = 0; i < length; i++)
			{
				if (filterExpr[i] == '(')
				{
					num4++;
				}
				if (filterExpr[i] == ')')
				{
					num4--;
				}
			}
			if (num4 > 0)
			{
				throw new LdapLocalException("MISSING_RIGHT_PAREN", 87);
			}
			if (num4 < 0)
			{
				throw new LdapLocalException("MISSING_LEFT_PAREN", 87);
			}
			ft = new FilterTokenizer(this, filterExpr);
			return parseFilter();
		}

		private Asn1Tagged parseFilter()
		{
			ft.getLeftParen();
			Asn1Tagged result = parseFilterComp();
			ft.getRightParen();
			return result;
		}

		private Asn1Tagged parseFilterComp()
		{
			Asn1Tagged result = null;
			int opOrAttr = ft.OpOrAttr;
			switch (opOrAttr)
			{
			case 0:
			case 1:
				result = new Asn1Tagged(new Asn1Identifier(2, constructed: true, opOrAttr), parseFilterList(), explicit_Renamed: false);
				break;
			case 2:
				result = new Asn1Tagged(new Asn1Identifier(2, constructed: true, opOrAttr), parseFilter(), explicit_Renamed: true);
				break;
			default:
			{
				int filterType = ft.FilterType;
				string value = ft.Value;
				switch (filterType)
				{
				case 5:
				case 6:
				case 8:
					result = new Asn1Tagged(new Asn1Identifier(2, constructed: true, filterType), new RfcAttributeValueAssertion(new RfcAttributeDescription(ft.Attr), new RfcAssertionValue(unescapeString(value))), explicit_Renamed: false);
					break;
				case 3:
					if (value.Equals("*"))
					{
						result = new Asn1Tagged(new Asn1Identifier(2, constructed: false, 7), new RfcAttributeDescription(ft.Attr), explicit_Renamed: false);
					}
					else if (value.IndexOf('*') != -1)
					{
						SupportClass.Tokenizer tokenizer2 = new SupportClass.Tokenizer(value, "*", retDel: true);
						Asn1SequenceOf asn1SequenceOf = new Asn1SequenceOf(5);
						int count = tokenizer2.Count;
						int num = 0;
						string text4 = new StringBuilder("").ToString();
						while (tokenizer2.HasMoreTokens())
						{
							string text5 = tokenizer2.NextToken();
							num++;
							if (text5.Equals("*"))
							{
								if (text4.Equals(text5))
								{
									asn1SequenceOf.add(new Asn1Tagged(new Asn1Identifier(2, constructed: false, 1), new RfcLdapString(unescapeString("")), explicit_Renamed: false));
								}
							}
							else if (num == 1)
							{
								asn1SequenceOf.add(new Asn1Tagged(new Asn1Identifier(2, constructed: false, 0), new RfcLdapString(unescapeString(text5)), explicit_Renamed: false));
							}
							else if (num < count)
							{
								asn1SequenceOf.add(new Asn1Tagged(new Asn1Identifier(2, constructed: false, 1), new RfcLdapString(unescapeString(text5)), explicit_Renamed: false));
							}
							else
							{
								asn1SequenceOf.add(new Asn1Tagged(new Asn1Identifier(2, constructed: false, 2), new RfcLdapString(unescapeString(text5)), explicit_Renamed: false));
							}
							text4 = text5;
						}
						result = new Asn1Tagged(new Asn1Identifier(2, constructed: true, 4), new RfcSubstringFilter(new RfcAttributeDescription(ft.Attr), asn1SequenceOf), explicit_Renamed: false);
					}
					else
					{
						result = new Asn1Tagged(new Asn1Identifier(2, constructed: true, 3), new RfcAttributeValueAssertion(new RfcAttributeDescription(ft.Attr), new RfcAssertionValue(unescapeString(value))), explicit_Renamed: false);
					}
					break;
				case 9:
				{
					string text = null;
					string text2 = null;
					bool flag = false;
					SupportClass.Tokenizer tokenizer = new SupportClass.Tokenizer(ft.Attr, ":");
					bool flag2 = true;
					while (tokenizer.HasMoreTokens())
					{
						string text3 = tokenizer.NextToken().Trim();
						if (flag2 && !text3.Equals(":"))
						{
							text = text3;
						}
						else if (text3.Equals("dn"))
						{
							flag = true;
						}
						else if (!text3.Equals(":"))
						{
							text2 = text3;
						}
						flag2 = false;
					}
					result = new Asn1Tagged(new Asn1Identifier(2, constructed: true, 9), new RfcMatchingRuleAssertion((text2 == null) ? null : new RfcMatchingRuleId(text2), (text == null) ? null : new RfcAttributeDescription(text), new RfcAssertionValue(unescapeString(value)), (!flag) ? null : new Asn1Boolean(content: true)), explicit_Renamed: false);
					break;
				}
				}
				break;
			}
			}
			return result;
		}

		private Asn1SetOf parseFilterList()
		{
			Asn1SetOf asn1SetOf = new Asn1SetOf();
			asn1SetOf.add(parseFilter());
			while (ft.peekChar() == '(')
			{
				asn1SetOf.add(parseFilter());
			}
			return asn1SetOf;
		}

		internal static int hex2int(char c)
		{
			if (c < '0' || c > '9')
			{
				if (c < 'A' || c > 'F')
				{
					if (c < 'a' || c > 'f')
					{
						return -1;
					}
					return c - 97 + 10;
				}
				return c - 65 + 10;
			}
			return c - 48;
		}

		private sbyte[] unescapeString(string string_Renamed)
		{
			sbyte[] array = new sbyte[string_Renamed.Length * 3];
			bool flag = false;
			bool flag2 = false;
			int length = string_Renamed.Length;
			char[] array2 = new char[1];
			char c = '\0';
			int i = 0;
			int num = 0;
			for (; i < length; i++)
			{
				char c2 = string_Renamed[i];
				if (flag)
				{
					int num2;
					if ((num2 = hex2int(c2)) < 0)
					{
						throw new LdapLocalException("INVALID_ESCAPE", new object[1] { c2 }, 87);
					}
					if (flag2)
					{
						c = (char)(num2 << 4);
						flag2 = false;
					}
					else
					{
						c = (char)(c | (ushort)num2);
						array[num++] = (sbyte)c;
						flag2 = (flag = false);
					}
					continue;
				}
				if (c2 == '\\')
				{
					flag2 = (flag = true);
					continue;
				}
				try
				{
					sbyte[] array3;
					if ((c2 >= '\u0001' && c2 <= '\'') || (c2 >= '+' && c2 <= '[') || c2 >= ']')
					{
						if (c2 <= '\u007f')
						{
							array[num++] = (sbyte)c2;
						}
						else
						{
							array2[0] = c2;
							array3 = SupportClass.ToSByteArray(Encoding.GetEncoding("utf-8").GetBytes(new string(array2)));
							Array.Copy(array3, 0, array, num, array3.Length);
							num += array3.Length;
						}
						flag = false;
						continue;
					}
					string text = "";
					array2[0] = c2;
					array3 = SupportClass.ToSByteArray(Encoding.GetEncoding("utf-8").GetBytes(new string(array2)));
					foreach (sbyte b in array3)
					{
						text = ((b < 0 || b >= 16) ? (text + "\\" + Convert.ToString(b & 0xFF, 16)) : (text + "\\0" + Convert.ToString(b & 0xFF, 16)));
					}
					throw new LdapLocalException("INVALID_CHAR_IN_FILTER", new object[2] { c2, text }, 87);
				}
				catch (IOException)
				{
					throw new SystemException("UTF-8 String encoding not supported by JVM");
				}
			}
			if (flag2 || flag)
			{
				throw new LdapLocalException("SHORT_ESCAPE", 87);
			}
			sbyte[] array4 = new sbyte[num];
			Array.Copy(array, 0, array4, 0, num);
			array = null;
			return array4;
		}

		private void addObject(Asn1Object current)
		{
			if (filterStack == null)
			{
				filterStack = new Stack();
			}
			if (choiceValue() == null)
			{
				ChoiceValue = current;
			}
			else
			{
				Asn1Tagged asn1Tagged = (Asn1Tagged)filterStack.Peek();
				Asn1Object asn1Object = asn1Tagged.taggedValue();
				if (asn1Object == null)
				{
					asn1Tagged.TaggedValue = current;
					filterStack.Push(current);
				}
				else if (asn1Object is Asn1SetOf)
				{
					((Asn1SetOf)asn1Object).add(current);
				}
				else if (asn1Object is Asn1Set)
				{
					((Asn1Set)asn1Object).add(current);
				}
				else if (asn1Object.getIdentifier().Tag == 2)
				{
					throw new LdapLocalException("Attemp to create more than one 'not' sub-filter", 87);
				}
			}
			int tag = current.getIdentifier().Tag;
			if (tag == 0 || tag == 1 || tag == 2)
			{
				filterStack.Push(current);
			}
		}

		public virtual void startSubstrings(string attrName)
		{
			finalFound = false;
			Asn1SequenceOf asn1SequenceOf = new Asn1SequenceOf(5);
			Asn1Object current = new Asn1Tagged(new Asn1Identifier(2, constructed: true, 4), new RfcSubstringFilter(new RfcAttributeDescription(attrName), asn1SequenceOf), explicit_Renamed: false);
			addObject(current);
			SupportClass.StackPush(filterStack, asn1SequenceOf);
		}

		[CLSCompliant(false)]
		public virtual void addSubstring(int type, sbyte[] value_Renamed)
		{
			try
			{
				Asn1SequenceOf asn1SequenceOf = (Asn1SequenceOf)filterStack.Peek();
				if (type != 0 && type != 1 && type != 2)
				{
					throw new LdapLocalException("Attempt to add an invalid substring type", 87);
				}
				if (type == 0 && asn1SequenceOf.size() != 0)
				{
					throw new LdapLocalException("Attempt to add an initial substring match after the first substring", 87);
				}
				if (finalFound)
				{
					throw new LdapLocalException("Attempt to add a substring match after a final substring match", 87);
				}
				if (type == 2)
				{
					finalFound = true;
				}
				asn1SequenceOf.add(new Asn1Tagged(new Asn1Identifier(2, constructed: false, type), new RfcLdapString(value_Renamed), explicit_Renamed: false));
			}
			catch (InvalidCastException)
			{
				throw new LdapLocalException("A call to addSubstring occured without calling startSubstring", 87);
			}
		}

		public virtual void endSubstrings()
		{
			try
			{
				finalFound = false;
				if (((Asn1SequenceOf)filterStack.Peek()).size() == 0)
				{
					throw new LdapLocalException("Empty substring filter", 87);
				}
			}
			catch (InvalidCastException)
			{
				throw new LdapLocalException("Missmatched ending of substrings", 87);
			}
			filterStack.Pop();
		}

		[CLSCompliant(false)]
		public virtual void addAttributeValueAssertion(int rfcType, string attrName, sbyte[] value_Renamed)
		{
			if (filterStack != null && filterStack.Count != 0 && filterStack.Peek() is Asn1SequenceOf)
			{
				throw new LdapLocalException("Cannot insert an attribute assertion in a substring", 87);
			}
			if (rfcType != 3 && rfcType != 5 && rfcType != 6 && rfcType != 8)
			{
				throw new LdapLocalException("Invalid filter type for AttributeValueAssertion", 87);
			}
			Asn1Object current = new Asn1Tagged(new Asn1Identifier(2, constructed: true, rfcType), new RfcAttributeValueAssertion(new RfcAttributeDescription(attrName), new RfcAssertionValue(value_Renamed)), explicit_Renamed: false);
			addObject(current);
		}

		public virtual void addPresent(string attrName)
		{
			Asn1Object current = new Asn1Tagged(new Asn1Identifier(2, constructed: false, 7), new RfcAttributeDescription(attrName), explicit_Renamed: false);
			addObject(current);
		}

		[CLSCompliant(false)]
		public virtual void addExtensibleMatch(string matchingRule, string attrName, sbyte[] value_Renamed, bool useDNMatching)
		{
			Asn1Object current = new Asn1Tagged(new Asn1Identifier(2, constructed: true, 9), new RfcMatchingRuleAssertion((matchingRule == null) ? null : new RfcMatchingRuleId(matchingRule), (attrName == null) ? null : new RfcAttributeDescription(attrName), new RfcAssertionValue(value_Renamed), (!useDNMatching) ? null : new Asn1Boolean(content: true)), explicit_Renamed: false);
			addObject(current);
		}

		public virtual void startNestedFilter(int rfcType)
		{
			Asn1Object current;
			switch (rfcType)
			{
			case 0:
			case 1:
				current = new Asn1Tagged(new Asn1Identifier(2, constructed: true, rfcType), new Asn1SetOf(), explicit_Renamed: false);
				break;
			case 2:
				current = new Asn1Tagged(new Asn1Identifier(2, constructed: true, rfcType), null, explicit_Renamed: true);
				break;
			default:
				throw new LdapLocalException("Attempt to create a nested filter other than AND, OR or NOT", 87);
			}
			addObject(current);
		}

		public virtual void endNestedFilter(int rfcType)
		{
			if (rfcType == 2)
			{
				filterStack.Pop();
			}
			if (((Asn1Object)filterStack.Peek()).getIdentifier().Tag != rfcType)
			{
				throw new LdapLocalException("Missmatched ending of nested filter", 87);
			}
			filterStack.Pop();
		}

		public virtual IEnumerator getFilterIterator()
		{
			return new FilterIterator(this, (Asn1Tagged)choiceValue());
		}

		public virtual string filterToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringFilter(getFilterIterator(), stringBuilder);
			return stringBuilder.ToString();
		}

		private static void stringFilter(IEnumerator itr, StringBuilder filter)
		{
			int num = -1;
			filter.Append('(');
			while (itr.MoveNext())
			{
				object current = itr.Current;
				if (current is int)
				{
					switch ((int)current)
					{
					case 0:
						filter.Append('&');
						break;
					case 1:
						filter.Append('|');
						break;
					case 2:
						filter.Append('!');
						break;
					case 3:
					{
						filter.Append((string)itr.Current);
						filter.Append('=');
						sbyte[] value_Renamed4 = (sbyte[])itr.Current;
						filter.Append(byteString(value_Renamed4));
						break;
					}
					case 5:
					{
						filter.Append((string)itr.Current);
						filter.Append(">=");
						sbyte[] value_Renamed3 = (sbyte[])itr.Current;
						filter.Append(byteString(value_Renamed3));
						break;
					}
					case 6:
					{
						filter.Append((string)itr.Current);
						filter.Append("<=");
						sbyte[] value_Renamed2 = (sbyte[])itr.Current;
						filter.Append(byteString(value_Renamed2));
						break;
					}
					case 7:
						filter.Append((string)itr.Current);
						filter.Append("=*");
						break;
					case 8:
					{
						filter.Append((string)itr.Current);
						filter.Append("~=");
						sbyte[] value_Renamed = (sbyte[])itr.Current;
						filter.Append(byteString(value_Renamed));
						break;
					}
					case 9:
					{
						string value = (string)itr.Current;
						filter.Append((string)itr.Current);
						filter.Append(':');
						filter.Append(value);
						filter.Append(":=");
						filter.Append((string)itr.Current);
						break;
					}
					case 4:
					{
						filter.Append((string)itr.Current);
						filter.Append('=');
						bool flag = false;
						while (itr.MoveNext())
						{
							switch ((int)itr.Current)
							{
							case 0:
								filter.Append((string)itr.Current);
								filter.Append('*');
								flag = false;
								break;
							case 1:
								if (flag)
								{
									filter.Append('*');
								}
								filter.Append((string)itr.Current);
								filter.Append('*');
								flag = false;
								break;
							case 2:
								if (flag)
								{
									filter.Append('*');
								}
								filter.Append((string)itr.Current);
								break;
							}
						}
						break;
					}
					}
				}
				else if (current is IEnumerator)
				{
					stringFilter((IEnumerator)current, filter);
				}
			}
			filter.Append(')');
		}

		private static string byteString(sbyte[] value_Renamed)
		{
			string text = null;
			if (Base64.isValidUTF8(value_Renamed, isUCS2Only: true))
			{
				try
				{
					return new string(Encoding.GetEncoding("utf-8").GetChars(SupportClass.ToByteArray(value_Renamed)));
				}
				catch (IOException ex)
				{
					throw new SystemException("Default JVM does not support UTF-8 encoding" + ex);
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < value_Renamed.Length; i++)
			{
				if (value_Renamed[i] >= 0)
				{
					stringBuilder.Append("\\0");
					stringBuilder.Append(Convert.ToString(value_Renamed[i], 16));
				}
				else
				{
					stringBuilder.Append("\\" + Convert.ToString(value_Renamed[i], 16).Substring(6));
				}
			}
			return stringBuilder.ToString();
		}
	}
}
