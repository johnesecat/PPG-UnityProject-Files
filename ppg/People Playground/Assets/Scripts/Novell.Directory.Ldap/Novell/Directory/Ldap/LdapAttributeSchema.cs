using System;
using System.Collections;
using System.IO;
using System.Text;
using Novell.Directory.Ldap.Utilclass;

namespace Novell.Directory.Ldap
{
	public class LdapAttributeSchema : LdapSchemaElement
	{
		private string syntaxString;

		private bool single;

		private string superior;

		private string equality;

		private string ordering;

		private string substring;

		private bool collective;

		private bool userMod = true;

		private int usage;

		public const int USER_APPLICATIONS = 0;

		public const int DIRECTORY_OPERATION = 1;

		public const int DISTRIBUTED_OPERATION = 2;

		public const int DSA_OPERATION = 3;

		public virtual string SyntaxString => syntaxString;

		public virtual string Superior => superior;

		public virtual bool SingleValued => single;

		public virtual string EqualityMatchingRule => equality;

		public virtual string OrderingMatchingRule => ordering;

		public virtual string SubstringMatchingRule => substring;

		public virtual bool Collective => collective;

		public virtual bool UserModifiable => userMod;

		public virtual int Usage => usage;

		private void InitBlock()
		{
			usage = 0;
		}

		public LdapAttributeSchema(string[] names, string oid, string description, string syntaxString, bool single, string superior, bool obsolete, string equality, string ordering, string substring, bool collective, bool isUserModifiable, int usage)
			: base(LdapSchema.schemaTypeNames[0])
		{
			InitBlock();
			base.names = names;
			base.oid = oid;
			base.description = description;
			base.obsolete = obsolete;
			this.syntaxString = syntaxString;
			this.single = single;
			this.equality = equality;
			this.ordering = ordering;
			this.substring = substring;
			this.collective = collective;
			userMod = isUserModifiable;
			this.usage = usage;
			this.superior = superior;
			base.Value = formatString();
		}

		public LdapAttributeSchema(string raw)
			: base(LdapSchema.schemaTypeNames[0])
		{
			InitBlock();
			try
			{
				SchemaParser schemaParser = new SchemaParser(raw);
				if (schemaParser.Names != null)
				{
					names = schemaParser.Names;
				}
				if (schemaParser.ID != null)
				{
					oid = schemaParser.ID;
				}
				if (schemaParser.Description != null)
				{
					description = schemaParser.Description;
				}
				if (schemaParser.Syntax != null)
				{
					syntaxString = schemaParser.Syntax;
				}
				if (schemaParser.Superior != null)
				{
					superior = schemaParser.Superior;
				}
				single = schemaParser.Single;
				obsolete = schemaParser.Obsolete;
				IEnumerator qualifiers = schemaParser.Qualifiers;
				while (qualifiers.MoveNext())
				{
					AttributeQualifier attributeQualifier = (AttributeQualifier)qualifiers.Current;
					setQualifier(attributeQualifier.Name, attributeQualifier.Values);
				}
				base.Value = formatString();
			}
			catch (IOException ex)
			{
				throw new SystemException(ex.ToString());
			}
		}

		protected internal override string formatString()
		{
			StringBuilder stringBuilder = new StringBuilder("( ");
			string iD;
			if ((iD = ID) != null)
			{
				stringBuilder.Append(iD);
			}
			string[] array = Names;
			if (array != null)
			{
				stringBuilder.Append(" NAME ");
				if (array.Length == 1)
				{
					stringBuilder.Append("'" + array[0] + "'");
				}
				else
				{
					stringBuilder.Append("( ");
					for (int i = 0; i < array.Length; i++)
					{
						stringBuilder.Append(" '" + array[i] + "'");
					}
					stringBuilder.Append(" )");
				}
			}
			if ((iD = Description) != null)
			{
				stringBuilder.Append(" DESC ");
				stringBuilder.Append("'" + iD + "'");
			}
			if (Obsolete)
			{
				stringBuilder.Append(" OBSOLETE");
			}
			if ((iD = Superior) != null)
			{
				stringBuilder.Append(" SUP ");
				stringBuilder.Append("'" + iD + "'");
			}
			if ((iD = EqualityMatchingRule) != null)
			{
				stringBuilder.Append(" EQUALITY ");
				stringBuilder.Append("'" + iD + "'");
			}
			if ((iD = OrderingMatchingRule) != null)
			{
				stringBuilder.Append(" ORDERING ");
				stringBuilder.Append("'" + iD + "'");
			}
			if ((iD = SubstringMatchingRule) != null)
			{
				stringBuilder.Append(" SUBSTR ");
				stringBuilder.Append("'" + iD + "'");
			}
			if ((iD = SyntaxString) != null)
			{
				stringBuilder.Append(" SYNTAX ");
				stringBuilder.Append(iD);
			}
			if (SingleValued)
			{
				stringBuilder.Append(" SINGLE-VALUE");
			}
			if (Collective)
			{
				stringBuilder.Append(" COLLECTIVE");
			}
			if (!UserModifiable)
			{
				stringBuilder.Append(" NO-USER-MODIFICATION");
			}
			int num;
			if ((num = Usage) != 0)
			{
				switch (num)
				{
				case 1:
					stringBuilder.Append(" USAGE directoryOperation");
					break;
				case 2:
					stringBuilder.Append(" USAGE distributedOperation");
					break;
				case 3:
					stringBuilder.Append(" USAGE dSAOperation");
					break;
				}
			}
			IEnumerator qualifierNames = QualifierNames;
			while (qualifierNames.MoveNext())
			{
				iD = (string)qualifierNames.Current;
				if (iD == null)
				{
					continue;
				}
				stringBuilder.Append(" " + iD);
				array = getQualifier(iD);
				if (array != null)
				{
					if (array.Length > 1)
					{
						stringBuilder.Append("(");
					}
					for (int j = 0; j < array.Length; j++)
					{
						stringBuilder.Append(" '" + array[j] + "'");
					}
					if (array.Length > 1)
					{
						stringBuilder.Append(" )");
					}
				}
			}
			stringBuilder.Append(" )");
			return stringBuilder.ToString();
		}
	}
}
