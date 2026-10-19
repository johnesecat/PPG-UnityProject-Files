using System;
using System.Globalization;
using Novell.Directory.Ldap.Utilclass;

namespace Novell.Directory.Ldap
{
	public class LdapException : Exception
	{
		private int resultCode;

		private string messageOrKey;

		private object[] arguments;

		private string matchedDN;

		private Exception rootException;

		private string serverMessage;

		public const int SUCCESS = 0;

		public const int OPERATIONS_ERROR = 1;

		public const int PROTOCOL_ERROR = 2;

		public const int TIME_LIMIT_EXCEEDED = 3;

		public const int SIZE_LIMIT_EXCEEDED = 4;

		public const int COMPARE_FALSE = 5;

		public const int COMPARE_TRUE = 6;

		public const int AUTH_METHOD_NOT_SUPPORTED = 7;

		public const int STRONG_AUTH_REQUIRED = 8;

		public const int Ldap_PARTIAL_RESULTS = 9;

		public const int REFERRAL = 10;

		public const int ADMIN_LIMIT_EXCEEDED = 11;

		public const int UNAVAILABLE_CRITICAL_EXTENSION = 12;

		public const int CONFIDENTIALITY_REQUIRED = 13;

		public const int SASL_BIND_IN_PROGRESS = 14;

		public const int NO_SUCH_ATTRIBUTE = 16;

		public const int UNDEFINED_ATTRIBUTE_TYPE = 17;

		public const int INAPPROPRIATE_MATCHING = 18;

		public const int CONSTRAINT_VIOLATION = 19;

		public const int ATTRIBUTE_OR_VALUE_EXISTS = 20;

		public const int INVALID_ATTRIBUTE_SYNTAX = 21;

		public const int NO_SUCH_OBJECT = 32;

		public const int ALIAS_PROBLEM = 33;

		public const int INVALID_DN_SYNTAX = 34;

		public const int IS_LEAF = 35;

		public const int ALIAS_DEREFERENCING_PROBLEM = 36;

		public const int INAPPROPRIATE_AUTHENTICATION = 48;

		public const int INVALID_CREDENTIALS = 49;

		public const int INSUFFICIENT_ACCESS_RIGHTS = 50;

		public const int BUSY = 51;

		public const int UNAVAILABLE = 52;

		public const int UNWILLING_TO_PERFORM = 53;

		public const int LOOP_DETECT = 54;

		public const int NAMING_VIOLATION = 64;

		public const int OBJECT_CLASS_VIOLATION = 65;

		public const int NOT_ALLOWED_ON_NONLEAF = 66;

		public const int NOT_ALLOWED_ON_RDN = 67;

		public const int ENTRY_ALREADY_EXISTS = 68;

		public const int OBJECT_CLASS_MODS_PROHIBITED = 69;

		public const int AFFECTS_MULTIPLE_DSAS = 71;

		public const int OTHER = 80;

		public const int SERVER_DOWN = 81;

		public const int LOCAL_ERROR = 82;

		public const int ENCODING_ERROR = 83;

		public const int DECODING_ERROR = 84;

		public const int Ldap_TIMEOUT = 85;

		public const int AUTH_UNKNOWN = 86;

		public const int FILTER_ERROR = 87;

		public const int USER_CANCELLED = 88;

		public const int NO_MEMORY = 90;

		public const int CONNECT_ERROR = 91;

		public const int Ldap_NOT_SUPPORTED = 92;

		public const int CONTROL_NOT_FOUND = 93;

		public const int NO_RESULTS_RETURNED = 94;

		public const int MORE_RESULTS_TO_RETURN = 95;

		public const int CLIENT_LOOP = 96;

		public const int REFERRAL_LIMIT_EXCEEDED = 97;

		public const int INVALID_RESPONSE = 100;

		public const int AMBIGUOUS_RESPONSE = 101;

		public const int TLS_NOT_SUPPORTED = 112;

		public const int SSL_HANDSHAKE_FAILED = 113;

		public const int SSL_PROVIDER_NOT_FOUND = 114;

		public virtual string LdapErrorMessage
		{
			get
			{
				if (serverMessage != null && serverMessage.Length == 0)
				{
					return null;
				}
				return serverMessage;
			}
		}

		public virtual Exception Cause => rootException;

		public virtual int ResultCode => resultCode;

		public virtual string MatchedDN => matchedDN;

		public override string Message => resultCodeToString();

		public LdapException()
		{
		}

		public LdapException(string messageOrKey, int resultCode, string serverMsg)
			: this(messageOrKey, null, resultCode, serverMsg, null, null)
		{
		}

		public LdapException(string messageOrKey, object[] arguments, int resultCode, string serverMsg)
			: this(messageOrKey, arguments, resultCode, serverMsg, null, null)
		{
		}

		public LdapException(string messageOrKey, int resultCode, string serverMsg, Exception rootException)
			: this(messageOrKey, null, resultCode, serverMsg, null, rootException)
		{
		}

		public LdapException(string messageOrKey, object[] arguments, int resultCode, string serverMsg, Exception rootException)
			: this(messageOrKey, arguments, resultCode, serverMsg, null, rootException)
		{
		}

		public LdapException(string messageOrKey, int resultCode, string serverMsg, string matchedDN)
			: this(messageOrKey, null, resultCode, serverMsg, matchedDN, null)
		{
		}

		public LdapException(string messageOrKey, object[] arguments, int resultCode, string serverMsg, string matchedDN)
			: this(messageOrKey, arguments, resultCode, serverMsg, matchedDN, null)
		{
		}

		internal LdapException(string messageOrKey, object[] arguments, int resultCode, string serverMsg, string matchedDN, Exception rootException)
		{
			this.messageOrKey = messageOrKey;
			this.arguments = arguments;
			this.resultCode = resultCode;
			this.rootException = rootException;
			this.matchedDN = matchedDN;
			serverMessage = serverMsg;
		}

		public virtual string resultCodeToString()
		{
			return ResourcesHandler.getResultString(resultCode);
		}

		public static string resultCodeToString(int code)
		{
			return ResourcesHandler.getResultString(code);
		}

		public virtual string resultCodeToString(CultureInfo locale)
		{
			return ResourcesHandler.getResultString(resultCode, locale);
		}

		public static string resultCodeToString(int code, CultureInfo locale)
		{
			return ResourcesHandler.getResultString(code, locale);
		}

		public override string ToString()
		{
			return getExceptionString("LdapException");
		}

		internal virtual string getExceptionString(string exception)
		{
			string text = ResourcesHandler.getMessage("TOSTRING", new object[4]
			{
				exception,
				base.Message,
				resultCode,
				resultCodeToString()
			});
			if (text.ToUpper().Equals("TOSTRING".ToUpper()))
			{
				text = exception + ": (" + resultCode + ") " + resultCodeToString();
			}
			if (serverMessage != null && serverMessage.Length != 0)
			{
				string text2 = ResourcesHandler.getMessage("SERVER_MSG", new object[2] { exception, serverMessage });
				if (text2.ToUpper().Equals("SERVER_MSG".ToUpper()))
				{
					text2 = exception + ": Server Message: " + serverMessage;
				}
				text = text + "\n" + text2;
			}
			if (matchedDN != null)
			{
				string text2 = ResourcesHandler.getMessage("MATCHED_DN", new object[2] { exception, matchedDN });
				if (text2.ToUpper().Equals("MATCHED_DN".ToUpper()))
				{
					text2 = exception + ": Matched DN: " + matchedDN;
				}
				text = text + "\n" + text2;
			}
			if (rootException != null)
			{
				text = text + "\n" + rootException.ToString();
			}
			return text;
		}
	}
}
