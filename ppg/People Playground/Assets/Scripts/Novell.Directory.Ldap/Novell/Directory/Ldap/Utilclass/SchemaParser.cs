using System.Collections;
using System.IO;
using System.Text;

namespace Novell.Directory.Ldap.Utilclass
{
	public class SchemaParser
	{
		internal string rawString;

		internal string[] names;

		internal string id;

		internal string description;

		internal string syntax;

		internal string superior;

		internal string nameForm;

		internal string objectClass;

		internal string[] superiors;

		internal string[] required;

		internal string[] optional;

		internal string[] auxiliary;

		internal string[] precluded;

		internal string[] applies;

		internal bool single;

		internal bool obsolete;

		internal string equality;

		internal string ordering;

		internal string substring;

		internal bool collective;

		internal bool userMod = true;

		internal int usage;

		internal int type = -1;

		internal int result;

		internal ArrayList qualifiers;

		public virtual string RawString
		{
			get
			{
				return rawString;
			}
			set
			{
				rawString = value;
			}
		}

		public virtual string[] Names => names;

		public virtual IEnumerator Qualifiers => qualifiers.GetEnumerator();

		public virtual string ID => id;

		public virtual string Description => description;

		public virtual string Syntax => syntax;

		public virtual string Superior => superior;

		public virtual bool Single => single;

		public virtual bool Obsolete => obsolete;

		public virtual string Equality => equality;

		public virtual string Ordering => ordering;

		public virtual string Substring => substring;

		public virtual bool Collective => collective;

		public virtual bool UserMod => userMod;

		public virtual int Usage => usage;

		public virtual int Type => type;

		public virtual string[] Superiors => superiors;

		public virtual string[] Required => required;

		public virtual string[] Optional => optional;

		public virtual string[] Auxiliary => auxiliary;

		public virtual string[] Precluded => precluded;

		public virtual string[] Applies => applies;

		public virtual string NameForm => nameForm;

		public virtual string ObjectClass => nameForm;

		private void InitBlock()
		{
			usage = 0;
			qualifiers = new ArrayList();
		}

		public SchemaParser(string aString)
		{
			InitBlock();
			int num;
			if ((num = aString.IndexOf('\\')) != -1)
			{
				StringBuilder stringBuilder = new StringBuilder(aString.Substring(0, num));
				for (int i = num; i < aString.Length; i++)
				{
					stringBuilder.Append(aString[i]);
					if (aString[i] == '\\')
					{
						stringBuilder.Append('\\');
					}
				}
				rawString = stringBuilder.ToString();
			}
			else
			{
				rawString = aString;
			}
			SchemaTokenCreator schemaTokenCreator = new SchemaTokenCreator(new StringReader(rawString));
			schemaTokenCreator.OrdinaryCharacter(46);
			schemaTokenCreator.OrdinaryCharacters(48, 57);
			schemaTokenCreator.OrdinaryCharacter(123);
			schemaTokenCreator.OrdinaryCharacter(125);
			schemaTokenCreator.OrdinaryCharacter(95);
			schemaTokenCreator.OrdinaryCharacter(59);
			schemaTokenCreator.WordCharacters(46, 57);
			schemaTokenCreator.WordCharacters(123, 125);
			schemaTokenCreator.WordCharacters(95, 95);
			schemaTokenCreator.WordCharacters(59, 59);
			try
			{
				if (-1 == schemaTokenCreator.nextToken() || schemaTokenCreator.lastttype != 40)
				{
					return;
				}
				if (-3 == schemaTokenCreator.nextToken())
				{
					id = schemaTokenCreator.StringValue;
				}
				while (-1 != schemaTokenCreator.nextToken())
				{
					if (schemaTokenCreator.lastttype != -3)
					{
						continue;
					}
					if (schemaTokenCreator.StringValue.ToUpper().Equals("NAME".ToUpper()))
					{
						if (schemaTokenCreator.nextToken() == 39)
						{
							names = new string[1];
							names[0] = schemaTokenCreator.StringValue;
						}
						else
						{
							if (schemaTokenCreator.lastttype != 40)
							{
								continue;
							}
							ArrayList arrayList = new ArrayList();
							while (schemaTokenCreator.nextToken() == 39)
							{
								if (schemaTokenCreator.StringValue != null)
								{
									arrayList.Add(schemaTokenCreator.StringValue);
								}
							}
							if (arrayList.Count > 0)
							{
								names = new string[arrayList.Count];
								SupportClass.ArrayListSupport.ToArray(arrayList, names);
							}
						}
					}
					else if (schemaTokenCreator.StringValue.ToUpper().Equals("DESC".ToUpper()))
					{
						if (schemaTokenCreator.nextToken() == 39)
						{
							description = schemaTokenCreator.StringValue;
						}
					}
					else if (schemaTokenCreator.StringValue.ToUpper().Equals("SYNTAX".ToUpper()))
					{
						result = schemaTokenCreator.nextToken();
						if (result == -3 || result == 39)
						{
							syntax = schemaTokenCreator.StringValue;
						}
					}
					else if (schemaTokenCreator.StringValue.ToUpper().Equals("EQUALITY".ToUpper()))
					{
						if (schemaTokenCreator.nextToken() == -3)
						{
							equality = schemaTokenCreator.StringValue;
						}
					}
					else if (schemaTokenCreator.StringValue.ToUpper().Equals("ORDERING".ToUpper()))
					{
						if (schemaTokenCreator.nextToken() == -3)
						{
							ordering = schemaTokenCreator.StringValue;
						}
					}
					else if (schemaTokenCreator.StringValue.ToUpper().Equals("SUBSTR".ToUpper()))
					{
						if (schemaTokenCreator.nextToken() == -3)
						{
							substring = schemaTokenCreator.StringValue;
						}
					}
					else if (schemaTokenCreator.StringValue.ToUpper().Equals("FORM".ToUpper()))
					{
						if (schemaTokenCreator.nextToken() == -3)
						{
							nameForm = schemaTokenCreator.StringValue;
						}
					}
					else if (schemaTokenCreator.StringValue.ToUpper().Equals("OC".ToUpper()))
					{
						if (schemaTokenCreator.nextToken() == -3)
						{
							objectClass = schemaTokenCreator.StringValue;
						}
					}
					else if (schemaTokenCreator.StringValue.ToUpper().Equals("SUP".ToUpper()))
					{
						ArrayList arrayList2 = new ArrayList();
						schemaTokenCreator.nextToken();
						if (schemaTokenCreator.lastttype == 40)
						{
							schemaTokenCreator.nextToken();
							while (schemaTokenCreator.lastttype != 41)
							{
								if (schemaTokenCreator.lastttype != 36)
								{
									arrayList2.Add(schemaTokenCreator.StringValue);
								}
								schemaTokenCreator.nextToken();
							}
						}
						else
						{
							arrayList2.Add(schemaTokenCreator.StringValue);
							superior = schemaTokenCreator.StringValue;
						}
						if (arrayList2.Count > 0)
						{
							superiors = new string[arrayList2.Count];
							SupportClass.ArrayListSupport.ToArray(arrayList2, superiors);
						}
					}
					else if (schemaTokenCreator.StringValue.ToUpper().Equals("SINGLE-VALUE".ToUpper()))
					{
						single = true;
					}
					else if (schemaTokenCreator.StringValue.ToUpper().Equals("OBSOLETE".ToUpper()))
					{
						obsolete = true;
					}
					else if (schemaTokenCreator.StringValue.ToUpper().Equals("COLLECTIVE".ToUpper()))
					{
						collective = true;
					}
					else if (schemaTokenCreator.StringValue.ToUpper().Equals("NO-USER-MODIFICATION".ToUpper()))
					{
						userMod = false;
					}
					else if (schemaTokenCreator.StringValue.ToUpper().Equals("MUST".ToUpper()))
					{
						ArrayList arrayList3 = new ArrayList();
						schemaTokenCreator.nextToken();
						if (schemaTokenCreator.lastttype == 40)
						{
							schemaTokenCreator.nextToken();
							while (schemaTokenCreator.lastttype != 41)
							{
								if (schemaTokenCreator.lastttype != 36)
								{
									arrayList3.Add(schemaTokenCreator.StringValue);
								}
								schemaTokenCreator.nextToken();
							}
						}
						else
						{
							arrayList3.Add(schemaTokenCreator.StringValue);
						}
						if (arrayList3.Count > 0)
						{
							required = new string[arrayList3.Count];
							SupportClass.ArrayListSupport.ToArray(arrayList3, required);
						}
					}
					else if (schemaTokenCreator.StringValue.ToUpper().Equals("MAY".ToUpper()))
					{
						ArrayList arrayList4 = new ArrayList();
						schemaTokenCreator.nextToken();
						if (schemaTokenCreator.lastttype == 40)
						{
							schemaTokenCreator.nextToken();
							while (schemaTokenCreator.lastttype != 41)
							{
								if (schemaTokenCreator.lastttype != 36)
								{
									arrayList4.Add(schemaTokenCreator.StringValue);
								}
								schemaTokenCreator.nextToken();
							}
						}
						else
						{
							arrayList4.Add(schemaTokenCreator.StringValue);
						}
						if (arrayList4.Count > 0)
						{
							optional = new string[arrayList4.Count];
							SupportClass.ArrayListSupport.ToArray(arrayList4, optional);
						}
					}
					else if (schemaTokenCreator.StringValue.ToUpper().Equals("NOT".ToUpper()))
					{
						ArrayList arrayList5 = new ArrayList();
						schemaTokenCreator.nextToken();
						if (schemaTokenCreator.lastttype == 40)
						{
							schemaTokenCreator.nextToken();
							while (schemaTokenCreator.lastttype != 41)
							{
								if (schemaTokenCreator.lastttype != 36)
								{
									arrayList5.Add(schemaTokenCreator.StringValue);
								}
								schemaTokenCreator.nextToken();
							}
						}
						else
						{
							arrayList5.Add(schemaTokenCreator.StringValue);
						}
						if (arrayList5.Count > 0)
						{
							precluded = new string[arrayList5.Count];
							SupportClass.ArrayListSupport.ToArray(arrayList5, precluded);
						}
					}
					else if (schemaTokenCreator.StringValue.ToUpper().Equals("AUX".ToUpper()))
					{
						ArrayList arrayList6 = new ArrayList();
						schemaTokenCreator.nextToken();
						if (schemaTokenCreator.lastttype == 40)
						{
							schemaTokenCreator.nextToken();
							while (schemaTokenCreator.lastttype != 41)
							{
								if (schemaTokenCreator.lastttype != 36)
								{
									arrayList6.Add(schemaTokenCreator.StringValue);
								}
								schemaTokenCreator.nextToken();
							}
						}
						else
						{
							arrayList6.Add(schemaTokenCreator.StringValue);
						}
						if (arrayList6.Count > 0)
						{
							auxiliary = new string[arrayList6.Count];
							SupportClass.ArrayListSupport.ToArray(arrayList6, auxiliary);
						}
					}
					else if (schemaTokenCreator.StringValue.ToUpper().Equals("ABSTRACT".ToUpper()))
					{
						type = 0;
					}
					else if (schemaTokenCreator.StringValue.ToUpper().Equals("STRUCTURAL".ToUpper()))
					{
						type = 1;
					}
					else if (schemaTokenCreator.StringValue.ToUpper().Equals("AUXILIARY".ToUpper()))
					{
						type = 2;
					}
					else if (schemaTokenCreator.StringValue.ToUpper().Equals("USAGE".ToUpper()))
					{
						if (schemaTokenCreator.nextToken() == -3)
						{
							string stringValue = schemaTokenCreator.StringValue;
							if (stringValue.ToUpper().Equals("directoryOperation".ToUpper()))
							{
								usage = 1;
							}
							else if (stringValue.ToUpper().Equals("distributedOperation".ToUpper()))
							{
								usage = 2;
							}
							else if (stringValue.ToUpper().Equals("dSAOperation".ToUpper()))
							{
								usage = 3;
							}
							else if (stringValue.ToUpper().Equals("userApplications".ToUpper()))
							{
								usage = 0;
							}
						}
					}
					else if (schemaTokenCreator.StringValue.ToUpper().Equals("APPLIES".ToUpper()))
					{
						ArrayList arrayList7 = new ArrayList();
						schemaTokenCreator.nextToken();
						if (schemaTokenCreator.lastttype == 40)
						{
							schemaTokenCreator.nextToken();
							while (schemaTokenCreator.lastttype != 41)
							{
								if (schemaTokenCreator.lastttype != 36)
								{
									arrayList7.Add(schemaTokenCreator.StringValue);
								}
								schemaTokenCreator.nextToken();
							}
						}
						else
						{
							arrayList7.Add(schemaTokenCreator.StringValue);
						}
						if (arrayList7.Count > 0)
						{
							applies = new string[arrayList7.Count];
							SupportClass.ArrayListSupport.ToArray(arrayList7, applies);
						}
					}
					else
					{
						string stringValue = schemaTokenCreator.StringValue;
						AttributeQualifier attributeQualifier = parseQualifier(schemaTokenCreator, stringValue);
						if (attributeQualifier != null)
						{
							qualifiers.Add(attributeQualifier);
						}
					}
				}
			}
			catch (IOException ex)
			{
				throw ex;
			}
		}

		private AttributeQualifier parseQualifier(SchemaTokenCreator st, string name)
		{
			ArrayList arrayList = new ArrayList(5);
			try
			{
				if (st.nextToken() == 39)
				{
					arrayList.Add(st.StringValue);
				}
				else if (st.lastttype == 40)
				{
					while (st.nextToken() == 39)
					{
						arrayList.Add(st.StringValue);
					}
				}
			}
			catch (IOException ex)
			{
				throw ex;
			}
			string[] objects = new string[arrayList.Count];
			objects = (string[])SupportClass.ArrayListSupport.ToArray(arrayList, objects);
			return new AttributeQualifier(name, objects);
		}
	}
}
