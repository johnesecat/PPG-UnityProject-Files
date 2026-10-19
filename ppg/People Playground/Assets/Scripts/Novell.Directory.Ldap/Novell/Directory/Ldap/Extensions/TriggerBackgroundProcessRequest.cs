using System;

namespace Novell.Directory.Ldap.Extensions
{
	public class TriggerBackgroundProcessRequest : LdapExtendedOperation
	{
		public const int Ldap_BK_PROCESS_BKLINKER = 1;

		public const int Ldap_BK_PROCESS_JANITOR = 2;

		public const int Ldap_BK_PROCESS_LIMBER = 3;

		public const int Ldap_BK_PROCESS_SKULKER = 4;

		public const int Ldap_BK_PROCESS_SCHEMA_SYNC = 5;

		public const int Ldap_BK_PROCESS_PART_PURGE = 6;

		public TriggerBackgroundProcessRequest(int processID)
			: base(null, null)
		{
			switch (processID)
			{
			case 1:
				setID("2.16.840.1.113719.1.27.100.43");
				break;
			case 2:
				setID("2.16.840.1.113719.1.27.100.47");
				break;
			case 3:
				setID("2.16.840.1.113719.1.27.100.49");
				break;
			case 4:
				setID("2.16.840.1.113719.1.27.100.51");
				break;
			case 5:
				setID("2.16.840.1.113719.1.27.100.53");
				break;
			case 6:
				setID("2.16.840.1.113719.1.27.100.55");
				break;
			default:
				throw new ArgumentException("PARAM_ERROR");
			}
		}
	}
}
