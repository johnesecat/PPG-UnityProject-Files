using System.Collections;
using System.IO;
using System.Text;
using Novell.Directory.Ldap.Utilclass;

namespace Novell.Directory.Ldap
{
	public class LdapObjectClassSchema : LdapSchemaElement
	{
		internal string[] superiors;

		internal string[] required;

		internal string[] optional;

		internal int type = -1;

		public const int ABSTRACT = 0;

		public const int STRUCTURAL = 1;

		public const int AUXILIARY = 2;

		public virtual string[] Superiors => superiors;

		public virtual string[] RequiredAttributes => required;

		public virtual string[] OptionalAttributes => optional;

		public virtual int Type => type;

		public LdapObjectClassSchema(string[] names, string oid, string[] superiors, string description, string[] required, string[] optional, int type, bool obsolete)
			: base(LdapSchema.schemaTypeNames[1])
		{
			base.names = new string[names.Length];
			names.CopyTo(base.names, 0);
			base.oid = oid;
			base.description = description;
			this.type = type;
			base.obsolete = obsolete;
			if (superiors != null)
			{
				this.superiors = new string[superiors.Length];
				superiors.CopyTo(this.superiors, 0);
			}
			if (required != null)
			{
				this.required = new string[required.Length];
				required.CopyTo(this.required, 0);
			}
			if (optional != null)
			{
				this.optional = new string[optional.Length];
				optional.CopyTo(this.optional, 0);
			}
			base.Value = formatString();
		}

		public LdapObjectClassSchema(string raw)
			: base(LdapSchema.schemaTypeNames[1])
		{
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
				obsolete = schemaParser.Obsolete;
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
				if (schemaParser.Superiors != null)
				{
					superiors = new string[schemaParser.Superiors.Length];
					schemaParser.Superiors.CopyTo(superiors, 0);
				}
				type = schemaParser.Type;
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
			if ((array = Superiors) != null)
			{
				stringBuilder.Append(" SUP ");
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
			if (Type != -1)
			{
				if (Type == 0)
				{
					stringBuilder.Append(" ABSTRACT");
				}
				else if (Type == 2)
				{
					stringBuilder.Append(" AUXILIARY");
				}
				else if (Type == 1)
				{
					stringBuilder.Append(" STRUCTURAL");
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
					for (int m = 0; m < array2.Length; m++)
					{
						if (m > 0)
						{
							stringBuilder.Append(" ");
						}
						stringBuilder.Append("'" + array2[m] + "'");
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
