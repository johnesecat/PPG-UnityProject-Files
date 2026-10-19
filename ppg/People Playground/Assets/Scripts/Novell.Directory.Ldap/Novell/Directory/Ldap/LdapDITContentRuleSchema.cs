using System.Collections;
using System.IO;
using System.Text;
using Novell.Directory.Ldap.Utilclass;

namespace Novell.Directory.Ldap
{
	public class LdapDITContentRuleSchema : LdapSchemaElement
	{
		private string[] auxiliary = new string[1] { "" };

		private string[] required = new string[1] { "" };

		private string[] optional = new string[1] { "" };

		private string[] precluded = new string[1] { "" };

		public virtual string[] AuxiliaryClasses => auxiliary;

		public virtual string[] RequiredAttributes => required;

		public virtual string[] OptionalAttributes => optional;

		public virtual string[] PrecludedAttributes => precluded;

		public LdapDITContentRuleSchema(string[] names, string oid, string description, bool obsolete, string[] auxiliary, string[] required, string[] optional, string[] precluded)
			: base(LdapSchema.schemaTypeNames[4])
		{
			base.names = new string[names.Length];
			names.CopyTo(base.names, 0);
			base.oid = oid;
			base.description = description;
			base.obsolete = obsolete;
			this.auxiliary = auxiliary;
			this.required = required;
			this.optional = optional;
			this.precluded = precluded;
			base.Value = formatString();
		}

		public LdapDITContentRuleSchema(string raw)
			: base(LdapSchema.schemaTypeNames[4])
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
					oid = schemaParser.ID;
				}
				if (schemaParser.Description != null)
				{
					description = schemaParser.Description;
				}
				if (schemaParser.Auxiliary != null)
				{
					auxiliary = new string[schemaParser.Auxiliary.Length];
					schemaParser.Auxiliary.CopyTo(auxiliary, 0);
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
				if (schemaParser.Precluded != null)
				{
					precluded = new string[schemaParser.Precluded.Length];
					schemaParser.Precluded.CopyTo(precluded, 0);
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
			if ((array = AuxiliaryClasses) != null)
			{
				stringBuilder.Append(" AUX ");
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
			if ((array = RequiredAttributes) != null)
			{
				stringBuilder.Append(" MUST ");
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
			if ((array = OptionalAttributes) != null)
			{
				stringBuilder.Append(" MAY ");
				if (array.Length > 1)
				{
					stringBuilder.Append("( ");
				}
				for (int l = 0; l < array.Length; l++)
				{
					if (l > 0)
					{
						stringBuilder.Append(" $ ");
					}
					stringBuilder.Append(array[l]);
				}
				if (array.Length > 1)
				{
					stringBuilder.Append(" )");
				}
			}
			if ((array = PrecludedAttributes) != null)
			{
				stringBuilder.Append(" NOT ");
				if (array.Length > 1)
				{
					stringBuilder.Append("( ");
				}
				for (int m = 0; m < array.Length; m++)
				{
					if (m > 0)
					{
						stringBuilder.Append(" $ ");
					}
					stringBuilder.Append(array[m]);
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
					for (int n = 0; n < array2.Length; n++)
					{
						if (n > 0)
						{
							stringBuilder.Append(" ");
						}
						stringBuilder.Append("'" + array2[n] + "'");
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
