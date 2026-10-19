using System;
using System.Collections;
using Novell.Directory.Ldap.Utilclass;

namespace Novell.Directory.Ldap
{
	public class LdapSchema : LdapEntry
	{
		private Hashtable[] idTable;

		private Hashtable[] nameTable;

		internal static readonly string[] schemaTypeNames = new string[8] { "attributeTypes", "objectClasses", "ldapSyntaxes", "nameForms", "dITContentRules", "dITStructureRules", "matchingRules", "matchingRuleUse" };

		internal const int ATTRIBUTE = 0;

		internal const int OBJECT_CLASS = 1;

		internal const int SYNTAX = 2;

		internal const int NAME_FORM = 3;

		internal const int DITCONTENT = 4;

		internal const int DITSTRUCTURE = 5;

		internal const int MATCHING = 6;

		internal const int MATCHING_USE = 7;

		public virtual IEnumerator AttributeSchemas => new EnumeratedIterator(idTable[0].Values.GetEnumerator());

		public virtual IEnumerator DITContentRuleSchemas => new EnumeratedIterator(idTable[4].Values.GetEnumerator());

		public virtual IEnumerator DITStructureRuleSchemas => new EnumeratedIterator(idTable[5].Values.GetEnumerator());

		public virtual IEnumerator MatchingRuleSchemas => new EnumeratedIterator(idTable[6].Values.GetEnumerator());

		public virtual IEnumerator MatchingRuleUseSchemas => new EnumeratedIterator(idTable[7].Values.GetEnumerator());

		public virtual IEnumerator NameFormSchemas => new EnumeratedIterator(idTable[3].Values.GetEnumerator());

		public virtual IEnumerator ObjectClassSchemas => new EnumeratedIterator(idTable[1].Values.GetEnumerator());

		public virtual IEnumerator SyntaxSchemas => new EnumeratedIterator(idTable[2].Values.GetEnumerator());

		public virtual IEnumerator AttributeNames => new EnumeratedIterator(new SupportClass.SetSupport(nameTable[0].Keys).GetEnumerator());

		public virtual IEnumerator DITContentRuleNames => new EnumeratedIterator(new SupportClass.SetSupport(nameTable[4].Keys).GetEnumerator());

		public virtual IEnumerator DITStructureRuleNames => new EnumeratedIterator(new SupportClass.SetSupport(nameTable[5].Keys).GetEnumerator());

		public virtual IEnumerator MatchingRuleNames => new EnumeratedIterator(new SupportClass.SetSupport(nameTable[6].Keys).GetEnumerator());

		public virtual IEnumerator MatchingRuleUseNames => new EnumeratedIterator(new SupportClass.SetSupport(nameTable[7].Keys).GetEnumerator());

		public virtual IEnumerator NameFormNames => new EnumeratedIterator(new SupportClass.SetSupport(nameTable[3].Keys).GetEnumerator());

		public virtual IEnumerator ObjectClassNames => new EnumeratedIterator(new SupportClass.SetSupport(nameTable[1].Keys).GetEnumerator());

		private void InitBlock()
		{
			nameTable = new Hashtable[8];
			idTable = new Hashtable[8];
		}

		public LdapSchema(LdapEntry ent)
			: base(ent.DN, ent.getAttributeSet())
		{
			InitBlock();
			for (int i = 0; i < schemaTypeNames.Length; i++)
			{
				idTable[i] = new Hashtable();
				nameTable[i] = new Hashtable();
			}
			IEnumerator enumerator = base.getAttributeSet().GetEnumerator();
			while (enumerator.MoveNext())
			{
				LdapAttribute obj = (LdapAttribute)enumerator.Current;
				string name = obj.Name;
				IEnumerator stringValues = obj.StringValues;
				if (name.ToUpper().Equals(schemaTypeNames[1].ToUpper()))
				{
					while (stringValues.MoveNext())
					{
						string raw = (string)stringValues.Current;
						LdapObjectClassSchema element;
						try
						{
							element = new LdapObjectClassSchema(raw);
						}
						catch (Exception)
						{
							continue;
						}
						addElement(1, element);
					}
				}
				else if (name.ToUpper().Equals(schemaTypeNames[0].ToUpper()))
				{
					while (stringValues.MoveNext())
					{
						string raw = (string)stringValues.Current;
						LdapAttributeSchema element2;
						try
						{
							element2 = new LdapAttributeSchema(raw);
						}
						catch (Exception)
						{
							continue;
						}
						addElement(0, element2);
					}
				}
				else if (name.ToUpper().Equals(schemaTypeNames[2].ToUpper()))
				{
					while (stringValues.MoveNext())
					{
						string raw = (string)stringValues.Current;
						LdapSyntaxSchema element3 = new LdapSyntaxSchema(raw);
						addElement(2, element3);
					}
				}
				else if (name.ToUpper().Equals(schemaTypeNames[6].ToUpper()))
				{
					while (stringValues.MoveNext())
					{
						string raw = (string)stringValues.Current;
						LdapMatchingRuleSchema element4 = new LdapMatchingRuleSchema(raw, null);
						addElement(6, element4);
					}
				}
				else if (name.ToUpper().Equals(schemaTypeNames[7].ToUpper()))
				{
					while (stringValues.MoveNext())
					{
						string raw = (string)stringValues.Current;
						LdapMatchingRuleUseSchema element5 = new LdapMatchingRuleUseSchema(raw);
						addElement(7, element5);
					}
				}
				else if (name.ToUpper().Equals(schemaTypeNames[4].ToUpper()))
				{
					while (stringValues.MoveNext())
					{
						string raw = (string)stringValues.Current;
						LdapDITContentRuleSchema element6 = new LdapDITContentRuleSchema(raw);
						addElement(4, element6);
					}
				}
				else if (name.ToUpper().Equals(schemaTypeNames[5].ToUpper()))
				{
					while (stringValues.MoveNext())
					{
						string raw = (string)stringValues.Current;
						LdapDITStructureRuleSchema element7 = new LdapDITStructureRuleSchema(raw);
						addElement(5, element7);
					}
				}
				else if (name.ToUpper().Equals(schemaTypeNames[3].ToUpper()))
				{
					while (stringValues.MoveNext())
					{
						string raw = (string)stringValues.Current;
						LdapNameFormSchema element8 = new LdapNameFormSchema(raw);
						addElement(3, element8);
					}
				}
			}
		}

		private void addElement(int schemaType, LdapSchemaElement element)
		{
			SupportClass.PutElement(idTable[schemaType], element.ID, element);
			string[] names = element.Names;
			for (int i = 0; i < names.Length; i++)
			{
				SupportClass.PutElement(nameTable[schemaType], names[i].ToUpper(), element);
			}
		}

		private LdapSchemaElement getSchemaElement(int schemaType, string key)
		{
			if (key == null || key.ToUpper().Equals("".ToUpper()))
			{
				return null;
			}
			char c = key[0];
			if (c >= '0' && c <= '9')
			{
				return (LdapSchemaElement)idTable[schemaType][key];
			}
			return (LdapSchemaElement)nameTable[schemaType][key.ToUpper()];
		}

		public virtual LdapAttributeSchema getAttributeSchema(string name)
		{
			return (LdapAttributeSchema)getSchemaElement(0, name);
		}

		public virtual LdapDITContentRuleSchema getDITContentRuleSchema(string name)
		{
			return (LdapDITContentRuleSchema)getSchemaElement(4, name);
		}

		public virtual LdapDITStructureRuleSchema getDITStructureRuleSchema(string name)
		{
			return (LdapDITStructureRuleSchema)getSchemaElement(5, name);
		}

		public virtual LdapDITStructureRuleSchema getDITStructureRuleSchema(int ID)
		{
			return (LdapDITStructureRuleSchema)idTable[5][ID];
		}

		public virtual LdapMatchingRuleSchema getMatchingRuleSchema(string name)
		{
			return (LdapMatchingRuleSchema)getSchemaElement(6, name);
		}

		public virtual LdapMatchingRuleUseSchema getMatchingRuleUseSchema(string name)
		{
			return (LdapMatchingRuleUseSchema)getSchemaElement(7, name);
		}

		public virtual LdapNameFormSchema getNameFormSchema(string name)
		{
			return (LdapNameFormSchema)getSchemaElement(3, name);
		}

		public virtual LdapObjectClassSchema getObjectClassSchema(string name)
		{
			return (LdapObjectClassSchema)getSchemaElement(1, name);
		}

		public virtual LdapSyntaxSchema getSyntaxSchema(string oid)
		{
			return (LdapSyntaxSchema)getSchemaElement(2, oid);
		}

		private int getType(LdapSchemaElement element)
		{
			if (element is LdapAttributeSchema)
			{
				return 0;
			}
			if (element is LdapObjectClassSchema)
			{
				return 1;
			}
			if (element is LdapSyntaxSchema)
			{
				return 2;
			}
			if (element is LdapNameFormSchema)
			{
				return 3;
			}
			if (element is LdapMatchingRuleSchema)
			{
				return 6;
			}
			if (element is LdapMatchingRuleUseSchema)
			{
				return 7;
			}
			if (element is LdapDITContentRuleSchema)
			{
				return 4;
			}
			if (element is LdapDITStructureRuleSchema)
			{
				return 5;
			}
			throw new ArgumentException("The specified schema element type is not recognized");
		}
	}
}
