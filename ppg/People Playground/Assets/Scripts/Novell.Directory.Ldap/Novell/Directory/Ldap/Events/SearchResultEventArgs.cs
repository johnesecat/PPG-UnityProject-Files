using System.Text;

namespace Novell.Directory.Ldap.Events
{
	public class SearchResultEventArgs : LdapEventArgs
	{
		public LdapEntry Entry => ((LdapSearchResult)ldap_message).Entry;

		public SearchResultEventArgs(LdapMessage sourceMessage, EventClassifiers aClassification, LdapEventType aType)
			: base(sourceMessage, EventClassifiers.CLASSIFICATION_LDAP_PSEARCH, aType)
		{
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("[{0}:", GetType());
			stringBuilder.AppendFormat("(Classification={0})", eClassification);
			stringBuilder.AppendFormat("(Type={0})", getChangeTypeString());
			stringBuilder.AppendFormat("(EventInformation:{0})", getStringRepresentaionOfEventInformation());
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		private string getStringRepresentaionOfEventInformation()
		{
			StringBuilder stringBuilder = new StringBuilder();
			LdapSearchResult ldapSearchResult = (LdapSearchResult)ldap_message;
			stringBuilder.AppendFormat("(Entry={0})", ldapSearchResult.Entry);
			LdapControl[] controls = ldapSearchResult.Controls;
			if (controls != null)
			{
				stringBuilder.Append("(Controls=");
				int num = 0;
				LdapControl[] array = controls;
				foreach (LdapControl ldapControl in array)
				{
					stringBuilder.AppendFormat("(Control{0}={1})", ++num, ldapControl.ToString());
				}
				stringBuilder.Append(")");
			}
			return stringBuilder.ToString();
		}

		private string getChangeTypeString()
		{
			switch (eType)
			{
			case LdapEventType.LDAP_PSEARCH_ADD:
				return "ADD";
			case LdapEventType.LDAP_PSEARCH_DELETE:
				return "DELETE";
			case LdapEventType.LDAP_PSEARCH_MODIFY:
				return "MODIFY";
			case LdapEventType.LDAP_PSEARCH_MODDN:
				return "MODDN";
			default:
				return "No change type: " + eType;
			}
		}
	}
}
