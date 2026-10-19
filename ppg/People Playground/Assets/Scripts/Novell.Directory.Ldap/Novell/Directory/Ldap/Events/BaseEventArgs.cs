using System;

namespace Novell.Directory.Ldap.Events
{
	public class BaseEventArgs : EventArgs
	{
		protected LdapMessage ldap_message;

		public LdapMessage ContianedEventInformation => ldap_message;

		public BaseEventArgs(LdapMessage message)
		{
			ldap_message = message;
		}
	}
}
