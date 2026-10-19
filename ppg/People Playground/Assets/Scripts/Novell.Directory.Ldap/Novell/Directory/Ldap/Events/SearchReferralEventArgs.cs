namespace Novell.Directory.Ldap.Events
{
	public class SearchReferralEventArgs : LdapEventArgs
	{
		public SearchReferralEventArgs(LdapMessage sourceMessage, EventClassifiers aClassification, LdapEventType aType)
			: base(sourceMessage, EventClassifiers.CLASSIFICATION_LDAP_PSEARCH, LdapEventType.LDAP_PSEARCH_ANY)
		{
		}

		public string[] getUrls()
		{
			return ((LdapSearchResultReference)ldap_message).Referrals;
		}
	}
}
