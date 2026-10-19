using System.Collections;
using System.IO;
using System.Text;
using Novell.Directory.Ldap.Utilclass;

namespace Novell.Directory.Ldap
{
	public class LdapNameFormSchema : LdapSchemaElement
	{
		private string objectClass;

		private string[] required;

		private string[] optional;

		public virtual string ObjectClass => objectClass;

		public virtual string[] RequiredNamingAttributes => required;

		public virtual string[] OptionalNamingAttributes => optional;

		public LdapNameFormSchema(string[] names, string oid, string description, bool obsolete, string objectClass, string[] required, string[] optional)
			: base(LdapSchema.schemaTypeNames[3])
		{
			base.names = new string[names.Length];
			names.CopyTo(base.names, 0);
			base.oid = oid;
			base.description = description;
			base.obsolete = obsolete;
			this.objectClass = objectClass;
			this.required = new string[required.Length];
			required.CopyTo(this.required, 0);
			this.optional = new string[optional.Length];
			optional.CopyTo(this.optional, 0);
			base.Value = formatString();
		}

		public LdapNameFormSchema(string raw)
			: base(LdapSchema.schemaTypeNames[3])
		{
			obsolete = false;
			try
			{
				SchemaParser schemaParser = new SchemaParser(raw);
				if (schemaParser.Names != null)
				{
					names = new string[schemaParser.Names.Length];
					schemaParser.Names.CopyTo(names, 0);
				}
				if (schemaParser.ID != null)
				{
					oid = new StringBuilder(schemaParser.ID).ToString();
				}
				if (schemaParser.Description != null)
				{
					description = new StringBuilder(schemaParser.Description).ToString();
				}
				if (schemaParser.Required != null)
				{
					required = new string[schemaParser.Required.Length];
					schemaParser.Required.CopyTo(required, 0);
				}
				if (schemaParser.Optional != null)
				{
					optional = new string[schemaParser.Optional.Length];
					schemaParser.Optional.CopyTo(optional, 0);
				}
				if (schemaParser.ObjectClass != null)
				{
					objectClass = schemaParser.ObjectClass;
				}
				obsolete = schemaParser.Obsolete;
				IEnumerator qualifiers = schemaParser.Qualifiers;
				while (qualifiers.MoveNext())
				{
					AttributeQualifier attributeQualifier = (AttributeQualifier)qualifiers.Current;
					setQualifier(attributeQualifier.Name, attributeQualifier.Values);
				}
				base.Value = formatString();
			}
			catch (IOException)
			{
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
			if ((iD = ObjectClass) != null)
			{
				stringBuilder.Append(" OC ");
				stringBuilder.Append("'" + iD + "'");
			}
			if ((array = RequiredNamingAttributes) != null)
			{
				stringBuilder.Append(" MUST ");
				if (array.Length > 1)
				{
					stringBuilder.Append("( ");
				}
				for (int j = 0; j < array.Length; j++)
				{
					if (j > 0)
					{
						stringBuilder.Append(" $ ");
					}
					stringBuilder.Append(array[j]);
				}
				if (array.Length > 1)
				{
					stringBuilder.Append(" )");
				}
			}
			if ((array = OptionalNamingAttributes) != null)
			{
				stringBuilder.Append(" MAY ");
				if (array.Length > 1)
				{
					stringBuilder.Append("( ");
				}
				for (int k = 0; k < array.Length; k++)
				{
					if (k > 0)
					{
						stringBuilder.Append(" $ ");
					}
					stringBuilder.Append(array[k]);
				}
				if (array.Length > 1)
				{
					stringBuilder.Append(" )");
				}
			}
			IEnumerator qualifierNames;
			if ((qualifierNames = QualifierNames) != null)
			{
				while (qualifierNames.MoveNext())
				{
					string text = (string)qualifierNames.Current;
					stringBuilder.Append(" " + text + " ");
					string[] array2;
					if ((array2 = getQualifier(text)) == null)
					{
						continue;
					}
					if (array2.Length > 1)
					{
						stringBuilder.Append("( ");
					}
					for (int l = 0; l < array2.Length; l++)
					{
						if (l > 0)
						{
							stringBuilder.Append(" ");
						}
						stringBuilder.Append("'" + array2[l] + "'");
					}
					if (array2.Length > 1)
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
