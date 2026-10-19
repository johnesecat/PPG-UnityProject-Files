namespace Novell.Directory.Ldap.Extensions
{
	public class ReplicationConstants
	{
		public const string CREATE_NAMING_CONTEXT_REQ = "2.16.840.1.113719.1.27.100.3";

		public const string CREATE_NAMING_CONTEXT_RES = "2.16.840.1.113719.1.27.100.4";

		public const string MERGE_NAMING_CONTEXT_REQ = "2.16.840.1.113719.1.27.100.5";

		public const string MERGE_NAMING_CONTEXT_RES = "2.16.840.1.113719.1.27.100.6";

		public const string ADD_REPLICA_REQ = "2.16.840.1.113719.1.27.100.7";

		public const string ADD_REPLICA_RES = "2.16.840.1.113719.1.27.100.8";

		public const string REFRESH_SERVER_REQ = "2.16.840.1.113719.1.27.100.9";

		public const string REFRESH_SERVER_RES = "2.16.840.1.113719.1.27.100.10";

		public const string DELETE_REPLICA_REQ = "2.16.840.1.113719.1.27.100.11";

		public const string DELETE_REPLICA_RES = "2.16.840.1.113719.1.27.100.12";

		public const string NAMING_CONTEXT_COUNT_REQ = "2.16.840.1.113719.1.27.100.13";

		public const string NAMING_CONTEXT_COUNT_RES = "2.16.840.1.113719.1.27.100.14";

		public const string CHANGE_REPLICA_TYPE_REQ = "2.16.840.1.113719.1.27.100.15";

		public const string CHANGE_REPLICA_TYPE_RES = "2.16.840.1.113719.1.27.100.16";

		public const string GET_REPLICA_INFO_REQ = "2.16.840.1.113719.1.27.100.17";

		public const string GET_REPLICA_INFO_RES = "2.16.840.1.113719.1.27.100.18";

		public const string LIST_REPLICAS_REQ = "2.16.840.1.113719.1.27.100.19";

		public const string LIST_REPLICAS_RES = "2.16.840.1.113719.1.27.100.20";

		public const string RECEIVE_ALL_UPDATES_REQ = "2.16.840.1.113719.1.27.100.21";

		public const string RECEIVE_ALL_UPDATES_RES = "2.16.840.1.113719.1.27.100.22";

		public const string SEND_ALL_UPDATES_REQ = "2.16.840.1.113719.1.27.100.23";

		public const string SEND_ALL_UPDATES_RES = "2.16.840.1.113719.1.27.100.24";

		public const string NAMING_CONTEXT_SYNC_REQ = "2.16.840.1.113719.1.27.100.25";

		public const string NAMING_CONTEXT_SYNC_RES = "2.16.840.1.113719.1.27.100.26";

		public const string SCHEMA_SYNC_REQ = "2.16.840.1.113719.1.27.100.27";

		public const string SCHEMA_SYNC_RES = "2.16.840.1.113719.1.27.100.28";

		public const string ABORT_NAMING_CONTEXT_OP_REQ = "2.16.840.1.113719.1.27.100.29";

		public const string ABORT_NAMING_CONTEXT_OP_RES = "2.16.840.1.113719.1.27.100.30";

		public const string GET_IDENTITY_NAME_REQ = "2.16.840.1.113719.1.27.100.31";

		public const string GET_IDENTITY_NAME_RES = "2.16.840.1.113719.1.27.100.32";

		public const string GET_EFFECTIVE_PRIVILEGES_REQ = "2.16.840.1.113719.1.27.100.33";

		public const string GET_EFFECTIVE_PRIVILEGES_RES = "2.16.840.1.113719.1.27.100.34";

		public const string SET_REPLICATION_FILTER_REQ = "2.16.840.1.113719.1.27.100.35";

		public const string SET_REPLICATION_FILTER_RES = "2.16.840.1.113719.1.27.100.36";

		public const string GET_REPLICATION_FILTER_REQ = "2.16.840.1.113719.1.27.100.37";

		public const string GET_REPLICATION_FILTER_RES = "2.16.840.1.113719.1.27.100.38";

		public const string CREATE_ORPHAN_NAMING_CONTEXT_REQ = "2.16.840.1.113719.1.27.100.39";

		public const string CREATE_ORPHAN_NAMING_CONTEXT_RES = "2.16.840.1.113719.1.27.100.40";

		public const string REMOVE_ORPHAN_NAMING_CONTEXT_REQ = "2.16.840.1.113719.1.27.100.41";

		public const string REMOVE_ORPHAN_NAMING_CONTEXT_RES = "2.16.840.1.113719.1.27.100.42";

		public const string TRIGGER_BKLINKER_REQ = "2.16.840.1.113719.1.27.100.43";

		public const string TRIGGER_BKLINKER_RES = "2.16.840.1.113719.1.27.100.44";

		public const string TRIGGER_JANITOR_REQ = "2.16.840.1.113719.1.27.100.47";

		public const string TRIGGER_JANITOR_RES = "2.16.840.1.113719.1.27.100.48";

		public const string TRIGGER_LIMBER_REQ = "2.16.840.1.113719.1.27.100.49";

		public const string TRIGGER_LIMBER_RES = "2.16.840.1.113719.1.27.100.50";

		public const string TRIGGER_SKULKER_REQ = "2.16.840.1.113719.1.27.100.51";

		public const string TRIGGER_SKULKER_RES = "2.16.840.1.113719.1.27.100.52";

		public const string TRIGGER_SCHEMA_SYNC_REQ = "2.16.840.1.113719.1.27.100.53";

		public const string TRIGGER_SCHEMA_SYNC_RES = "2.16.840.1.113719.1.27.100.54";

		public const string TRIGGER_PART_PURGE_REQ = "2.16.840.1.113719.1.27.100.55";

		public const string TRIGGER_PART_PURGE_RES = "2.16.840.1.113719.1.27.100.56";

		public const int Ldap_ENSURE_SERVERS_UP = 1;

		public const int Ldap_RT_MASTER = 0;

		public const int Ldap_RT_SECONDARY = 1;

		public const int Ldap_RT_READONLY = 2;

		public const int Ldap_RT_SUBREF = 3;

		public const int Ldap_RT_SPARSE_WRITE = 4;

		public const int Ldap_RT_SPARSE_READ = 5;

		public const int Ldap_RS_ON = 0;

		public const int Ldap_RS_NEW_REPLICA = 1;

		public const int Ldap_RS_DYING_REPLICA = 2;

		public const int Ldap_RS_LOCKED = 3;

		public const int Ldap_RS_TRANSITION_ON = 6;

		public const int Ldap_RS_DEAD_REPLICA = 7;

		public const int Ldap_RS_BEGIN_ADD = 8;

		public const int Ldap_RS_MASTER_START = 11;

		public const int Ldap_RS_MASTER_DONE = 12;

		public const int Ldap_RS_SS_0 = 48;

		public const int Ldap_RS_SS_1 = 49;

		public const int Ldap_RS_JS_0 = 64;

		public const int Ldap_RS_JS_1 = 65;

		public const int Ldap_RS_JS_2 = 66;

		public const int Ldap_DS_FLAG_BUSY = 1;

		public const int Ldap_DS_FLAG_BOUNDARY = 2;
	}
}
