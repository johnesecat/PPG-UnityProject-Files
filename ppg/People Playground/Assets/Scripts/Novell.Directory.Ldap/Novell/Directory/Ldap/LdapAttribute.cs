using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using Novell.Directory.Ldap.Utilclass;

namespace Novell.Directory.Ldap
{
	public class LdapAttribute : ICloneable, IComparable
	{
		private class URLData
		{
			private LdapAttribute enclosingInstance;

			private int length;

			private sbyte[] data;

			public LdapAttribute Enclosing_Instance => enclosingInstance;

			private void InitBlock(LdapAttribute enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}

			public URLData(LdapAttribute enclosingInstance, sbyte[] data, int length)
			{
				InitBlock(enclosingInstance);
				this.length = length;
				this.data = data;
			}

			public int getLength()
			{
				return length;
			}

			public sbyte[] getData()
			{
				return data;
			}
		}

		private string name;

		private string baseName;

		private string[] subTypes;

		private object[] values;

		public virtual IEnumerator ByteValues => new ArrayEnumeration(ByteValueArray);

		public virtual IEnumerator StringValues => new ArrayEnumeration(StringValueArray);

		[CLSCompliant(false)]
		public virtual sbyte[][] ByteValueArray
		{
			get
			{
				if (values == null)
				{
					return new sbyte[0][];
				}
				int num = values.Length;
				sbyte[][] array = new sbyte[num][];
				int i = 0;
				for (int num2 = num; i < num2; i++)
				{
					array[i] = new sbyte[((sbyte[])values[i]).Length];
					Array.Copy((Array)values[i], 0, array[i], 0, array[i].Length);
				}
				return array;
			}
		}

		public virtual string[] StringValueArray
		{
			get
			{
				if (values == null)
				{
					return new string[0];
				}
				int num = values.Length;
				string[] array = new string[num];
				for (int i = 0; i < num; i++)
				{
					try
					{
						char[] chars = Encoding.GetEncoding("utf-8").GetChars(SupportClass.ToByteArray((sbyte[])values[i]));
						array[i] = new string(chars);
					}
					catch (IOException ex)
					{
						throw new SystemException(ex.ToString());
					}
				}
				return array;
			}
		}

		public virtual string StringValue
		{
			get
			{
				string result = null;
				if (values != null)
				{
					try
					{
						return new string(Encoding.GetEncoding("utf-8").GetChars(SupportClass.ToByteArray((sbyte[])values[0])));
					}
					catch (IOException ex)
					{
						throw new SystemException(ex.ToString());
					}
				}
				return result;
			}
		}

		[CLSCompliant(false)]
		public virtual sbyte[] ByteValue
		{
			get
			{
				sbyte[] array = null;
				if (values != null)
				{
					array = new sbyte[((sbyte[])values[0]).Length];
					Array.Copy((Array)values[0], 0, array, 0, array.Length);
				}
				return array;
			}
		}

		public virtual string LangSubtype
		{
			get
			{
				if (subTypes != null)
				{
					for (int i = 0; i < subTypes.Length; i++)
					{
						if (subTypes[i].StartsWith("lang-"))
						{
							return subTypes[i];
						}
					}
				}
				return null;
			}
		}

		public virtual string Name => name;

		protected internal virtual string Value
		{
			set
			{
				values = null;
				try
				{
					sbyte[] bytes = SupportClass.ToSByteArray(Encoding.GetEncoding("utf-8").GetBytes(value));
					add(bytes);
				}
				catch (IOException ex)
				{
					throw new SystemException(ex.ToString());
				}
			}
		}

		public LdapAttribute(LdapAttribute attr)
		{
			if (attr == null)
			{
				throw new ArgumentException("LdapAttribute class cannot be null");
			}
			name = attr.name;
			baseName = attr.baseName;
			if (attr.subTypes != null)
			{
				subTypes = new string[attr.subTypes.Length];
				Array.Copy(attr.subTypes, 0, subTypes, 0, subTypes.Length);
			}
			if (attr.values != null)
			{
				values = new object[attr.values.Length];
				Array.Copy(attr.values, 0, values, 0, values.Length);
			}
		}

		public LdapAttribute(string attrName)
		{
			if (attrName == null)
			{
				throw new ArgumentException("Attribute name cannot be null");
			}
			name = attrName;
			baseName = getBaseName(attrName);
			subTypes = getSubtypes(attrName);
		}

		[CLSCompliant(false)]
		public LdapAttribute(string attrName, sbyte[] attrBytes)
			: this(attrName)
		{
			if (attrBytes == null)
			{
				throw new ArgumentException("Attribute value cannot be null");
			}
			sbyte[] array = new sbyte[attrBytes.Length];
			Array.Copy(attrBytes, 0, array, 0, attrBytes.Length);
			add(array);
		}

		public LdapAttribute(string attrName, string attrString)
			: this(attrName)
		{
			if (attrString == null)
			{
				throw new ArgumentException("Attribute value cannot be null");
			}
			try
			{
				sbyte[] bytes = SupportClass.ToSByteArray(Encoding.GetEncoding("utf-8").GetBytes(attrString));
				add(bytes);
			}
			catch (IOException ex)
			{
				throw new SystemException(ex.ToString());
			}
		}

		public LdapAttribute(string attrName, string[] attrStrings)
			: this(attrName)
		{
			if (attrStrings == null)
			{
				throw new ArgumentException("Attribute values array cannot be null");
			}
			int i = 0;
			for (int num = attrStrings.Length; i < num; i++)
			{
				try
				{
					if (attrStrings[i] == null)
					{
						throw new ArgumentException("Attribute value at array index " + i + " cannot be null");
					}
					sbyte[] bytes = SupportClass.ToSByteArray(Encoding.GetEncoding("utf-8").GetBytes(attrStrings[i]));
					add(bytes);
				}
				catch (IOException ex)
				{
					throw new SystemException(ex.ToString());
				}
			}
		}

		public object Clone()
		{
			try
			{
				object obj = MemberwiseClone();
				if (values != null)
				{
					Array.Copy(values, 0, ((LdapAttribute)obj).values, 0, values.Length);
				}
				return obj;
			}
			catch (Exception)
			{
				throw new SystemException("Internal error, cannot create clone");
			}
		}

		public virtual void addValue(string attrString)
		{
			if (attrString == null)
			{
				throw new ArgumentException("Attribute value cannot be null");
			}
			try
			{
				sbyte[] bytes = SupportClass.ToSByteArray(Encoding.GetEncoding("utf-8").GetBytes(attrString));
				add(bytes);
			}
			catch (IOException ex)
			{
				throw new SystemException(ex.ToString());
			}
		}

		[CLSCompliant(false)]
		public virtual void addValue(sbyte[] attrBytes)
		{
			if (attrBytes == null)
			{
				throw new ArgumentException("Attribute value cannot be null");
			}
			add(attrBytes);
		}

		public virtual void addBase64Value(string attrString)
		{
			if (attrString == null)
			{
				throw new ArgumentException("Attribute value cannot be null");
			}
			add(Base64.decode(attrString));
		}

		public virtual void addBase64Value(StringBuilder attrString, int start, int end)
		{
			if (attrString == null)
			{
				throw new ArgumentException("Attribute value cannot be null");
			}
			add(Base64.decode(attrString, start, end));
		}

		public virtual void addBase64Value(char[] attrChars)
		{
			if (attrChars == null)
			{
				throw new ArgumentException("Attribute value cannot be null");
			}
			add(Base64.decode(attrChars));
		}

		public virtual void addURLValue(string url)
		{
			if (url == null)
			{
				throw new ArgumentException("Attribute URL cannot be null");
			}
			addURLValue(new Uri(url));
		}

		public virtual void addURLValue(Uri url)
		{
			if (url == null)
			{
				throw new ArgumentException("Attribute URL cannot be null");
			}
			try
			{
				Stream responseStream = WebRequest.Create(url).GetResponse().GetResponseStream();
				ArrayList arrayList = new ArrayList();
				sbyte[] target = new sbyte[4096];
				int num = 0;
				int num2;
				while ((num2 = SupportClass.ReadInput(responseStream, ref target, 0, 4096)) != -1)
				{
					arrayList.Add(new URLData(this, target, num2));
					target = new sbyte[4096];
					num += num2;
				}
				sbyte[] array = new sbyte[num];
				int num3 = 0;
				for (int i = 0; i < arrayList.Count; i++)
				{
					URLData obj = (URLData)arrayList[i];
					num2 = obj.getLength();
					Array.Copy(obj.getData(), 0, array, num3, num2);
					num3 += num2;
				}
				add(array);
			}
			catch (IOException ex)
			{
				throw new SystemException(ex.ToString());
			}
		}

		public virtual string getBaseName()
		{
			return baseName;
		}

		public static string getBaseName(string attrName)
		{
			if (attrName == null)
			{
				throw new ArgumentException("Attribute name cannot be null");
			}
			int num = attrName.IndexOf(';');
			if (-1 == num)
			{
				return attrName;
			}
			return attrName.Substring(0, num);
		}

		public virtual string[] getSubtypes()
		{
			return subTypes;
		}

		public static string[] getSubtypes(string attrName)
		{
			if (attrName == null)
			{
				throw new ArgumentException("Attribute name cannot be null");
			}
			SupportClass.Tokenizer tokenizer = new SupportClass.Tokenizer(attrName, ";");
			string[] array = null;
			int count = tokenizer.Count;
			if (count > 0)
			{
				tokenizer.NextToken();
				array = new string[count - 1];
				int num = 0;
				while (tokenizer.HasMoreTokens())
				{
					array[num++] = tokenizer.NextToken();
				}
			}
			return array;
		}

		public virtual bool hasSubtype(string subtype)
		{
			if (subtype == null)
			{
				throw new ArgumentException("subtype cannot be null");
			}
			if (subTypes != null)
			{
				for (int i = 0; i < subTypes.Length; i++)
				{
					if (subTypes[i].ToUpper().Equals(subtype.ToUpper()))
					{
						return true;
					}
				}
			}
			return false;
		}

		public virtual bool hasSubtypes(string[] subtypes)
		{
			if (subtypes == null)
			{
				throw new ArgumentException("subtypes cannot be null");
			}
			for (int i = 0; i < subtypes.Length; i++)
			{
				int num = 0;
				while (true)
				{
					if (num < subTypes.Length)
					{
						if (subTypes[num] == null)
						{
							throw new ArgumentException("subtype at array index " + i + " cannot be null");
						}
						if (subTypes[num].ToUpper().Equals(subtypes[i].ToUpper()))
						{
							break;
						}
						num++;
						continue;
					}
					return false;
				}
			}
			return true;
		}

		public virtual void removeValue(string attrString)
		{
			if (attrString == null)
			{
				throw new ArgumentException("Attribute value cannot be null");
			}
			try
			{
				sbyte[] attrBytes = SupportClass.ToSByteArray(Encoding.GetEncoding("utf-8").GetBytes(attrString));
				removeValue(attrBytes);
			}
			catch (IOException ex)
			{
				throw new SystemException(ex.ToString());
			}
		}

		[CLSCompliant(false)]
		public virtual void removeValue(sbyte[] attrBytes)
		{
			if (attrBytes == null)
			{
				throw new ArgumentException("Attribute value cannot be null");
			}
			for (int i = 0; i < values.Length; i++)
			{
				if (!equals(attrBytes, (sbyte[])values[i]))
				{
					continue;
				}
				if (i == 0 && 1 == values.Length)
				{
					values = null;
					break;
				}
				if (values.Length == 1)
				{
					values = null;
					break;
				}
				int num = values.Length - i - 1;
				object[] destinationArray = new object[values.Length - 1];
				if (i != 0)
				{
					Array.Copy(values, 0, destinationArray, 0, i);
				}
				if (num != 0)
				{
					Array.Copy(values, i + 1, destinationArray, i, num);
				}
				values = destinationArray;
				destinationArray = null;
				break;
			}
		}

		public virtual int size()
		{
			if (values != null)
			{
				return values.Length;
			}
			return 0;
		}

		public virtual int CompareTo(object attribute)
		{
			return name.CompareTo(((LdapAttribute)attribute).name);
		}

		private void add(sbyte[] bytes)
		{
			if (values == null)
			{
				values = new object[1] { bytes };
				return;
			}
			for (int i = 0; i < values.Length; i++)
			{
				if (equals(bytes, (sbyte[])values[i]))
				{
					return;
				}
			}
			object[] array = new object[values.Length + 1];
			Array.Copy(values, 0, array, 0, values.Length);
			array[values.Length] = bytes;
			values = array;
			array = null;
		}

		private bool equals(sbyte[] e1, sbyte[] e2)
		{
			if (e1 == e2)
			{
				return true;
			}
			if (e1 == null || e2 == null)
			{
				return false;
			}
			int num = e1.Length;
			if (e2.Length != num)
			{
				return false;
			}
			for (int i = 0; i < num; i++)
			{
				if (e1[i] != e2[i])
				{
					return false;
				}
			}
			return true;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("LdapAttribute: ");
			try
			{
				stringBuilder.Append("{type='" + name + "'");
				if (values != null)
				{
					stringBuilder.Append(", ");
					if (values.Length == 1)
					{
						stringBuilder.Append("value='");
					}
					else
					{
						stringBuilder.Append("values='");
					}
					for (int i = 0; i < values.Length; i++)
					{
						if (i != 0)
						{
							stringBuilder.Append("','");
						}
						if (((sbyte[])values[i]).Length != 0)
						{
							string text = new string(Encoding.GetEncoding("utf-8").GetChars(SupportClass.ToByteArray((sbyte[])values[i])));
							if (text.Length == 0)
							{
								stringBuilder.Append("<binary value, length:" + text.Length);
							}
							else
							{
								stringBuilder.Append(text);
							}
						}
					}
					stringBuilder.Append("'");
				}
				stringBuilder.Append("}");
			}
			catch (Exception ex)
			{
				throw new SystemException(ex.ToString());
			}
			return stringBuilder.ToString();
		}
	}
}
