using System.Runtime.InteropServices;

namespace Novell.Directory.Ldap
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct LdapDSConstants
	{
		public static readonly long LDAP_DS_ENTRY_BROWSE = 1L;

		public static readonly long LDAP_DS_ENTRY_ADD = 2L;

		public static readonly long LDAP_DS_ENTRY_DELETE = 4L;

		public static readonly long LDAP_DS_ENTRY_RENAME = 8L;

		public static readonly long LDAP_DS_ENTRY_SUPERVISOR = 16L;

		public static readonly long LDAP_DS_ENTRY_INHERIT_CTL = 64L;

		public static readonly long LDAP_DS_ATTR_COMPARE = 1L;

		public static readonly long LDAP_DS_ATTR_READ = 2L;

		public static readonly long LDAP_DS_ATTR_WRITE = 4L;

		public static readonly long LDAP_DS_ATTR_SELF = 8L;

		public static readonly long LDAP_DS_ATTR_SUPERVISOR = 32L;

		public static readonly long LDAP_DS_ATTR_INHERIT_CTL = 64L;

		public static readonly long LDAP_DS_DYNAMIC_ACL = 1073741824L;

		public static readonly int LDAP_DS_ALIAS_ENTRY = 1;

		public static readonly int LDAP_DS_PARTITION_ROOT = 2;

		public static readonly int LDAP_DS_CONTAINER_ENTRY = 4;

		public static readonly int LDAP_DS_CONTAINER_ALIAS = 8;

		public static readonly int LDAP_DS_MATCHES_LIST_FILTER = 16;

		public static readonly int LDAP_DS_REFERENCE_ENTRY = 32;

		public static readonly int LDAP_DS_40X_REFERENCE_ENTRY = 64;

		public static readonly int LDAP_DS_BACKLINKED = 128;

		public static readonly int LDAP_DS_NEW_ENTRY = 256;

		public static readonly int LDAP_DS_TEMPORARY_REFERENCE = 512;

		public static readonly int LDAP_DS_AUDITED = 1024;

		public static readonly int LDAP_DS_ENTRY_NOT_PRESENT = 2048;

		public static readonly int LDAP_DS_ENTRY_VERIFY_CTS = 4096;

		public static readonly int LDAP_DS_ENTRY_DAMAGED = 8192;
	}
}
