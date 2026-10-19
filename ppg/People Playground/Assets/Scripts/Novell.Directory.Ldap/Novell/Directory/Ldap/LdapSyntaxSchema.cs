using System;
using System.Collections;
using System.IO;
using System.Text;
using Novell.Directory.Ldap.Utilclass;

namespace Novell.Directory.Ldap
{
	public class LdapSyntaxSchema : LdapSchemaElement
	{
		public LdapSyntaxSchema(string oid, string description)
			: base(LdapSchema.schemaTypeNames[2])
		{
			base.oid = oid;
			base.description = description;
			base.Value = formatString();
		}

		public LdapSyntaxSchema(string raw)
			: base(LdapSchema.schemaTypeNames[2])
		{
			try
			{
				SchemaParser schemaParser = new SchemaParser(raw);
				if (schemaParser.ID != null)
				{
					oid = schemaParser.ID;
				}
				if (schemaParser.Description != null)
				{
					description = schemaParser.Description;
				}
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
			if ((iD = Description) != null)
			{
				stringBuilder.Append(" DESC ");
				stringBuilder.Append("'" + iD + "'");
			}
			IEnumerator qualifierNames;
			if ((qualifierNames = QualifierNames) != null)
			{
				while (qualifierNames.MoveNext())
				{
					string text = (string)qualifierNames.Current;
					stringBuilder.Append(" " + text + " ");
					string[] array;
					if ((array = getQualifier(text)) == null || array.Length <= 1)
					{
						continue;
					}
					stringBuilder.Append("( ");
					for (int i = 0; i < array.Length; i++)
					{
						if (i > 0)
						{
							stringBuilder.Append(" ");
						}
						stringBuilder.Append("'" + array[i] + "'");
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
