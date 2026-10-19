using System.Collections;
using System.IO;
using System.Text;
using Novell.Directory.Ldap.Utilclass;

namespace Novell.Directory.Ldap
{
	public class LdapDITStructureRuleSchema : LdapSchemaElement
	{
		private int ruleID;

		private string nameForm = "";

		private string[] superiorIDs = new string[1] { "" };

		public virtual int RuleID => ruleID;

		public virtual string NameForm => nameForm;

		public virtual string[] Superiors => superiorIDs;

		public LdapDITStructureRuleSchema(string[] names, int ruleID, string description, bool obsolete, string nameForm, string[] superiorIDs)
			: base(LdapSchema.schemaTypeNames[5])
		{
			base.names = new string[names.Length];
			names.CopyTo(base.names, 0);
			this.ruleID = ruleID;
			base.description = description;
			base.obsolete = obsolete;
			this.nameForm = nameForm;
			this.superiorIDs = superiorIDs;
			base.Value = formatString();
		}

		public LdapDITStructureRuleSchema(string raw)
			: base(LdapSchema.schemaTypeNames[5])
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
					ruleID = int.Parse(schemaParser.ID);
				}
				if (schemaParser.Description != null)
				{
					description = schemaParser.Description;
				}
				if (schemaParser.Superiors != null)
				{
					superiorIDs = new string[schemaParser.Superiors.Length];
					schemaParser.Superiors.CopyTo(superiorIDs, 0);
				}
				if (schemaParser.NameForm != null)
				{
					nameForm = schemaParser.NameForm;
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
			string value = RuleID.ToString();
			stringBuilder.Append(value);
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
			if ((value = Description) != null)
			{
				stringBuilder.Append(" DESC ");
				stringBuilder.Append("'" + value + "'");
			}
			if (Obsolete)
			{
				stringBuilder.Append(" OBSOLETE");
			}
			if ((value = NameForm) != null)
			{
				stringBuilder.Append(" FORM ");
				stringBuilder.Append("'" + value + "'");
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
						stringBuilder.Append(" ");
					}
					stringBuilder.Append(array[j]);
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
					for (int k = 0; k < array2.Length; k++)
					{
						if (k > 0)
						{
							stringBuilder.Append(" ");
						}
						stringBuilder.Append("'" + array2[k] + "'");
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
