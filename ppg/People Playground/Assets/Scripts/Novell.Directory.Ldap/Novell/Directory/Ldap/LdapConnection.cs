using System;
using System.Collections;
using System.IO;
using System.Text;
using Novell.Directory.Ldap.Utilclass;

namespace Novell.Directory.Ldap
{
	public class LdapConnection : ICloneable
	{
		private LdapSearchConstraints defSearchCons;

		private LdapControl[] responseCtls;

		private object responseCtlSemaphore;

		private Connection conn;

		private static object nameLock;

		private static int lConnNum;

		private string name;

		public const int SCOPE_BASE = 0;

		public const int SCOPE_ONE = 1;

		public const int SCOPE_SUB = 2;

		public const string NO_ATTRS = "1.1";

		public const string ALL_USER_ATTRS = "*";

		public const int Ldap_V3 = 3;

		public const int DEFAULT_PORT = 389;

		public const int DEFAULT_SSL_PORT = 636;

		public const string Ldap_PROPERTY_SDK = "version.sdk";

		public const string Ldap_PROPERTY_PROTOCOL = "version.protocol";

		public const string Ldap_PROPERTY_SECURITY = "version.security";

		public const string SERVER_SHUTDOWN_OID = "1.3.6.1.4.1.1466.20036";

		private const string START_TLS_OID = "1.3.6.1.4.1.1466.20037";

		public virtual int ProtocolVersion => conn.BindProperties?.ProtocolVersion ?? 3;

		public virtual string AuthenticationDN
		{
			get
			{
				BindProperties bindProperties = conn.BindProperties;
				if (bindProperties == null)
				{
					return null;
				}
				if (bindProperties.Anonymous)
				{
					return null;
				}
				return bindProperties.AuthenticationDN;
			}
		}

		public virtual string AuthenticationMethod
		{
			get
			{
				if (conn.BindProperties == null)
				{
					return "simple";
				}
				return conn.BindProperties.AuthenticationMethod;
			}
		}

		public virtual IDictionary SaslBindProperties
		{
			get
			{
				if (conn.BindProperties == null)
				{
					return null;
				}
				return conn.BindProperties.SaslBindProperties;
			}
		}

		public virtual object SaslBindCallbackHandler
		{
			get
			{
				if (conn.BindProperties == null)
				{
					return null;
				}
				return conn.BindProperties.SaslCallbackHandler;
			}
		}

		public virtual LdapConstraints Constraints
		{
			get
			{
				return (LdapConstraints)defSearchCons.Clone();
			}
			set
			{
				if (value is LdapSearchConstraints)
				{
					defSearchCons = (LdapSearchConstraints)value.Clone();
					return;
				}
				LdapSearchConstraints ldapSearchConstraints = (LdapSearchConstraints)defSearchCons.Clone();
				ldapSearchConstraints.HopLimit = value.HopLimit;
				ldapSearchConstraints.TimeLimit = value.TimeLimit;
				ldapSearchConstraints.setReferralHandler(value.getReferralHandler());
				ldapSearchConstraints.ReferralFollowing = value.ReferralFollowing;
				LdapControl[] controls = value.getControls();
				if (controls != null)
				{
					ldapSearchConstraints.setControls(controls);
				}
				Hashtable properties = ldapSearchConstraints.Properties;
				if (properties != null)
				{
					ldapSearchConstraints.Properties = properties;
				}
				defSearchCons = ldapSearchConstraints;
			}
		}

		public virtual string Host => conn.Host;

		public virtual int Port => conn.Port;

		public virtual LdapSearchConstraints SearchConstraints => (LdapSearchConstraints)defSearchCons.Clone();

		public bool SecureSocketLayer
		{
			get
			{
				return conn.Ssl;
			}
			set
			{
				conn.Ssl = value;
			}
		}

		public virtual bool Bound => conn.Bound;

		public virtual bool Connected => conn.Connected;

		public virtual bool TLS => conn.TLS;

		public virtual LdapControl[] ResponseControls
		{
			get
			{
				if (responseCtls == null)
				{
					return null;
				}
				LdapControl[] array = new LdapControl[responseCtls.Length];
				lock (responseCtlSemaphore)
				{
					for (int i = 0; i < responseCtls.Length; i++)
					{
						array[i] = (LdapControl)responseCtls[i].Clone();
					}
					return array;
				}
			}
		}

		internal virtual Connection Connection => conn;

		internal virtual string ConnectionName => name;

		public event CertificateValidationCallback UserDefinedServerCertValidationDelegate
		{
			add
			{
				conn.OnCertificateValidation += value;
			}
			remove
			{
				conn.OnCertificateValidation -= value;
			}
		}

		private void InitBlock()
		{
			defSearchCons = new LdapSearchConstraints();
			responseCtlSemaphore = new object();
		}

		public LdapConnection()
		{
			InitBlock();
			conn = new Connection();
		}

		public object Clone()
		{
			object obj;
			LdapConnection ldapConnection;
			try
			{
				obj = MemberwiseClone();
				ldapConnection = (LdapConnection)obj;
			}
			catch (Exception)
			{
				throw new SystemException("Internal error, cannot create clone");
			}
			ldapConnection.conn = conn;
			if (defSearchCons != null)
			{
				ldapConnection.defSearchCons = (LdapSearchConstraints)defSearchCons.Clone();
			}
			else
			{
				ldapConnection.defSearchCons = null;
			}
			if (responseCtls != null)
			{
				ldapConnection.responseCtls = new LdapControl[responseCtls.Length];
				for (int i = 0; i < responseCtls.Length; i++)
				{
					ldapConnection.responseCtls[i] = (LdapControl)responseCtls[i].Clone();
				}
			}
			else
			{
				ldapConnection.responseCtls = null;
			}
			conn.incrCloneCount();
			return obj;
		}

		~LdapConnection()
		{
			Disconnect(defSearchCons, how: false);
		}

		public virtual object getProperty(string name)
		{
			if (name.ToUpper().Equals("version.sdk".ToUpper()))
			{
				return Connection.sdk;
			}
			if (name.ToUpper().Equals("version.protocol".ToUpper()))
			{
				return Connection.protocol;
			}
			if (name.ToUpper().Equals("version.security".ToUpper()))
			{
				return Connection.security;
			}
			return null;
		}

		public virtual void AddUnsolicitedNotificationListener(LdapUnsolicitedNotificationListener listener)
		{
			if (listener != null)
			{
				conn.AddUnsolicitedNotificationListener(listener);
			}
		}

		public virtual void RemoveUnsolicitedNotificationListener(LdapUnsolicitedNotificationListener listener)
		{
			if (listener != null)
			{
				conn.RemoveUnsolicitedNotificationListener(listener);
			}
		}

		public virtual void startTLS()
		{
			LdapMessage ldapMessage = MakeExtendedOperation(new LdapExtendedOperation("1.3.6.1.4.1.1466.20037", null), null);
			int messageID = ldapMessage.MessageID;
			conn.acquireWriteSemaphore(messageID);
			try
			{
				if (!conn.areMessagesComplete())
				{
					throw new LdapLocalException("OUTSTANDING_OPERATIONS", 1);
				}
				conn.stopReaderOnReply(messageID);
				((LdapExtendedResponse)SendRequestToServer(ldapMessage, defSearchCons.TimeLimit, null, null).getResponse()).chkResultCode();
				conn.startTLS();
			}
			finally
			{
				conn.startReader();
				conn.freeWriteSemaphore(messageID);
			}
		}

		public virtual void stopTLS()
		{
			if (!TLS)
			{
				throw new LdapLocalException("NO_STARTTLS", 1);
			}
			int msgId = conn.acquireWriteSemaphore();
			try
			{
				if (!conn.areMessagesComplete())
				{
					throw new LdapLocalException("OUTSTANDING_OPERATIONS", 1);
				}
				conn.stopTLS();
			}
			finally
			{
				conn.freeWriteSemaphore(msgId);
				Connect(Host, Port);
			}
		}

		public virtual void Abandon(LdapSearchResults results)
		{
			Abandon(results, defSearchCons);
		}

		public virtual void Abandon(LdapSearchResults results, LdapConstraints cons)
		{
			results.Abandon();
		}

		public virtual void Abandon(int id)
		{
			Abandon(id, defSearchCons);
		}

		public virtual void Abandon(int id, LdapConstraints cons)
		{
			try
			{
				conn.getMessageAgent(id).Abandon(id, cons);
			}
			catch (FieldAccessException)
			{
			}
		}

		public virtual void Abandon(LdapMessageQueue queue)
		{
			Abandon(queue, defSearchCons);
		}

		public virtual void Abandon(LdapMessageQueue queue, LdapConstraints cons)
		{
			if (queue != null)
			{
				MessageAgent messageAgent = ((!(queue is LdapSearchQueue)) ? queue.MessageAgent : queue.MessageAgent);
				int[] messageIDs = messageAgent.MessageIDs;
				for (int i = 0; i < messageIDs.Length; i++)
				{
					messageAgent.Abandon(messageIDs[i], cons);
				}
			}
		}

		public virtual void Add(LdapEntry entry)
		{
			Add(entry, defSearchCons);
		}

		public virtual void Add(LdapEntry entry, LdapConstraints cons)
		{
			LdapResponseQueue ldapResponseQueue = Add(entry, null, cons);
			LdapResponse ldapResponse = (LdapResponse)ldapResponseQueue.getResponse();
			lock (responseCtlSemaphore)
			{
				responseCtls = ldapResponse.Controls;
			}
			chkResultCode(ldapResponseQueue, cons, ldapResponse);
		}

		public virtual LdapResponseQueue Add(LdapEntry entry, LdapResponseQueue queue)
		{
			return Add(entry, queue, defSearchCons);
		}

		public virtual LdapResponseQueue Add(LdapEntry entry, LdapResponseQueue queue, LdapConstraints cons)
		{
			if (cons == null)
			{
				cons = defSearchCons;
			}
			if (entry == null)
			{
				throw new ArgumentException("The LdapEntry parameter cannot be null");
			}
			if (entry.DN == null)
			{
				throw new ArgumentException("The DN value must be present in the LdapEntry object");
			}
			LdapMessage msg = new LdapAddRequest(entry, cons.getControls());
			return SendRequestToServer(msg, cons.TimeLimit, queue, null);
		}

		public virtual void Bind(string dn, string passwd)
		{
			Bind(dn, passwd, AuthenticationTypes.None);
		}

		public virtual void Bind(string dn, string passwd, AuthenticationTypes authenticationTypes)
		{
			Bind(3, dn, passwd, defSearchCons);
		}

		public virtual void Bind(int version, string dn, string passwd)
		{
			Bind(version, dn, passwd, defSearchCons);
		}

		public virtual void Bind(string dn, string passwd, LdapConstraints cons)
		{
			Bind(3, dn, passwd, cons);
		}

		public virtual void Bind(int version, string dn, string passwd, LdapConstraints cons)
		{
			sbyte[] passwd2 = null;
			if (passwd != null)
			{
				try
				{
					passwd2 = SupportClass.ToSByteArray(Encoding.GetEncoding("utf-8").GetBytes(passwd));
					passwd = null;
				}
				catch (IOException ex)
				{
					passwd = null;
					throw new SystemException(ex.ToString());
				}
			}
			Bind(version, dn, passwd2, cons);
		}

		[CLSCompliant(false)]
		public virtual void Bind(int version, string dn, sbyte[] passwd)
		{
			Bind(version, dn, passwd, defSearchCons);
		}

		[CLSCompliant(false)]
		public virtual void Bind(int version, string dn, sbyte[] passwd, LdapConstraints cons)
		{
			LdapResponseQueue ldapResponseQueue = Bind(version, dn, passwd, null, cons, null);
			LdapResponse ldapResponse = (LdapResponse)ldapResponseQueue.getResponse();
			if (ldapResponse != null)
			{
				lock (responseCtlSemaphore)
				{
					responseCtls = ldapResponse.Controls;
				}
				chkResultCode(ldapResponseQueue, cons, ldapResponse);
			}
		}

		[CLSCompliant(false)]
		public virtual LdapResponseQueue Bind(int version, string dn, sbyte[] passwd, LdapResponseQueue queue)
		{
			return Bind(version, dn, passwd, queue, defSearchCons, null);
		}

		[CLSCompliant(false)]
		public virtual LdapResponseQueue Bind(int version, string dn, sbyte[] passwd, LdapResponseQueue queue, LdapConstraints cons, string mech)
		{
			if (cons == null)
			{
				cons = defSearchCons;
			}
			dn = ((dn != null) ? dn.Trim() : "");
			if (passwd == null)
			{
				passwd = new sbyte[0];
			}
			bool anonymous = false;
			if (passwd.Length == 0)
			{
				anonymous = true;
				dn = "";
			}
			LdapMessage ldapMessage = new LdapBindRequest(version, dn, passwd, cons.getControls());
			int messageID = ldapMessage.MessageID;
			BindProperties bindProps = new BindProperties(version, dn, "simple", anonymous, null, null);
			if (!conn.Connected)
			{
				if (conn.Host == null)
				{
					throw new LdapException("CONNECTION_IMPOSSIBLE", 91, null);
				}
				conn.connect(conn.Host, conn.Port);
			}
			conn.acquireWriteSemaphore(messageID);
			return SendRequestToServer(ldapMessage, cons.TimeLimit, queue, bindProps);
		}

		public virtual bool Compare(string dn, LdapAttribute attr)
		{
			return Compare(dn, attr, defSearchCons);
		}

		public virtual bool Compare(string dn, LdapAttribute attr, LdapConstraints cons)
		{
			bool result = false;
			LdapResponseQueue ldapResponseQueue = Compare(dn, attr, null, cons);
			LdapResponse ldapResponse = (LdapResponse)ldapResponseQueue.getResponse();
			lock (responseCtlSemaphore)
			{
				responseCtls = ldapResponse.Controls;
			}
			if (ldapResponse.ResultCode == 6)
			{
				result = true;
			}
			else if (ldapResponse.ResultCode == 5)
			{
				result = false;
			}
			else
			{
				chkResultCode(ldapResponseQueue, cons, ldapResponse);
			}
			return result;
		}

		public virtual LdapResponseQueue Compare(string dn, LdapAttribute attr, LdapResponseQueue queue)
		{
			return Compare(dn, attr, queue, defSearchCons);
		}

		public virtual LdapResponseQueue Compare(string dn, LdapAttribute attr, LdapResponseQueue queue, LdapConstraints cons)
		{
			if (attr.size() != 1)
			{
				throw new ArgumentException("compare: Exactly one value must be present in the LdapAttribute");
			}
			if (dn == null)
			{
				throw new ArgumentException("compare: DN cannot be null");
			}
			if (cons == null)
			{
				cons = defSearchCons;
			}
			LdapMessage msg = new LdapCompareRequest(dn, attr.Name, attr.ByteValue, cons.getControls());
			return SendRequestToServer(msg, cons.TimeLimit, queue, null);
		}

		public virtual void Connect(string host, int port)
		{
			SupportClass.Tokenizer tokenizer = new SupportClass.Tokenizer(host, " ");
			string text = null;
			while (tokenizer.HasMoreTokens())
			{
				try
				{
					int port2 = port;
					text = tokenizer.NextToken();
					int num = text.IndexOf(':');
					if (num != -1 && num + 1 != text.Length)
					{
						try
						{
							port2 = int.Parse(text.Substring(num + 1));
							text = text.Substring(0, num);
						}
						catch (Exception)
						{
							throw new ArgumentException("INVALID_ADDRESS");
						}
					}
					conn = conn.destroyClone(apiCall: true);
					conn.connect(text, port2);
					return;
				}
				catch (LdapException ex2)
				{
					if (!tokenizer.HasMoreTokens())
					{
						throw ex2;
					}
				}
			}
		}

		public virtual void Delete(string dn)
		{
			Delete(dn, defSearchCons);
		}

		public virtual void Delete(string dn, LdapConstraints cons)
		{
			LdapResponseQueue ldapResponseQueue = Delete(dn, null, cons);
			LdapResponse ldapResponse = (LdapResponse)ldapResponseQueue.getResponse();
			lock (responseCtlSemaphore)
			{
				responseCtls = ldapResponse.Controls;
			}
			chkResultCode(ldapResponseQueue, cons, ldapResponse);
		}

		public virtual LdapResponseQueue Delete(string dn, LdapResponseQueue queue)
		{
			return Delete(dn, queue, defSearchCons);
		}

		public virtual LdapResponseQueue Delete(string dn, LdapResponseQueue queue, LdapConstraints cons)
		{
			if (dn == null)
			{
				throw new ArgumentException("DN_PARAM_ERROR");
			}
			if (cons == null)
			{
				cons = defSearchCons;
			}
			LdapMessage msg = new LdapDeleteRequest(dn, cons.getControls());
			return SendRequestToServer(msg, cons.TimeLimit, queue, null);
		}

		public virtual void Disconnect()
		{
			Disconnect(defSearchCons, how: true);
		}

		public virtual void Disconnect(LdapConstraints cons)
		{
			Disconnect(cons, how: true);
		}

		private void Disconnect(LdapConstraints cons, bool how)
		{
			conn = conn.destroyClone(how);
		}

		public virtual LdapExtendedResponse ExtendedOperation(LdapExtendedOperation op)
		{
			return ExtendedOperation(op, defSearchCons);
		}

		public virtual LdapExtendedResponse ExtendedOperation(LdapExtendedOperation op, LdapConstraints cons)
		{
			LdapResponseQueue ldapResponseQueue = ExtendedOperation(op, cons, null);
			LdapExtendedResponse ldapExtendedResponse = (LdapExtendedResponse)ldapResponseQueue.getResponse();
			lock (responseCtlSemaphore)
			{
				responseCtls = ldapExtendedResponse.Controls;
			}
			chkResultCode(ldapResponseQueue, cons, ldapExtendedResponse);
			return ldapExtendedResponse;
		}

		public virtual LdapResponseQueue ExtendedOperation(LdapExtendedOperation op, LdapResponseQueue queue)
		{
			return ExtendedOperation(op, defSearchCons, queue);
		}

		public virtual LdapResponseQueue ExtendedOperation(LdapExtendedOperation op, LdapConstraints cons, LdapResponseQueue queue)
		{
			if (cons == null)
			{
				cons = defSearchCons;
			}
			LdapMessage msg = MakeExtendedOperation(op, cons);
			return SendRequestToServer(msg, cons.TimeLimit, queue, null);
		}

		protected internal virtual LdapMessage MakeExtendedOperation(LdapExtendedOperation op, LdapConstraints cons)
		{
			if (cons == null)
			{
				cons = defSearchCons;
			}
			if (op.getID() == null)
			{
				throw new ArgumentException("OP_PARAM_ERROR");
			}
			return new LdapExtendedRequest(op, cons.getControls());
		}

		public virtual void Modify(string dn, LdapModification mod)
		{
			Modify(dn, mod, defSearchCons);
		}

		public virtual void Modify(string dn, LdapModification mod, LdapConstraints cons)
		{
			Modify(dn, new LdapModification[1] { mod }, cons);
		}

		public virtual void Modify(string dn, LdapModification[] mods)
		{
			Modify(dn, mods, defSearchCons);
		}

		public virtual void Modify(string dn, LdapModification[] mods, LdapConstraints cons)
		{
			LdapResponseQueue ldapResponseQueue = Modify(dn, mods, null, cons);
			LdapResponse ldapResponse = (LdapResponse)ldapResponseQueue.getResponse();
			lock (responseCtlSemaphore)
			{
				responseCtls = ldapResponse.Controls;
			}
			chkResultCode(ldapResponseQueue, cons, ldapResponse);
		}

		public virtual LdapResponseQueue Modify(string dn, LdapModification mod, LdapResponseQueue queue)
		{
			return Modify(dn, mod, queue, defSearchCons);
		}

		public virtual LdapResponseQueue Modify(string dn, LdapModification mod, LdapResponseQueue queue, LdapConstraints cons)
		{
			return Modify(dn, new LdapModification[1] { mod }, queue, cons);
		}

		public virtual LdapResponseQueue Modify(string dn, LdapModification[] mods, LdapResponseQueue queue)
		{
			return Modify(dn, mods, queue, defSearchCons);
		}

		public virtual LdapResponseQueue Modify(string dn, LdapModification[] mods, LdapResponseQueue queue, LdapConstraints cons)
		{
			if (dn == null)
			{
				throw new ArgumentException("DN_PARAM_ERROR");
			}
			if (cons == null)
			{
				cons = defSearchCons;
			}
			LdapMessage msg = new LdapModifyRequest(dn, mods, cons.getControls());
			return SendRequestToServer(msg, cons.TimeLimit, queue, null);
		}

		public virtual LdapEntry Read(string dn)
		{
			return Read(dn, defSearchCons);
		}

		public virtual LdapEntry Read(string dn, LdapSearchConstraints cons)
		{
			return Read(dn, null, cons);
		}

		public virtual LdapEntry Read(string dn, string[] attrs)
		{
			return Read(dn, attrs, defSearchCons);
		}

		public virtual LdapEntry Read(string dn, string[] attrs, LdapSearchConstraints cons)
		{
			LdapSearchResults ldapSearchResults = Search(dn, 0, null, attrs, typesOnly: false, cons);
			LdapEntry result = null;
			if (ldapSearchResults.hasMore())
			{
				result = ldapSearchResults.next();
				if (ldapSearchResults.hasMore())
				{
					throw new LdapLocalException("READ_MULTIPLE", 101);
				}
			}
			return result;
		}

		public static LdapEntry Read(LdapUrl toGet)
		{
			LdapConnection ldapConnection = new LdapConnection();
			ldapConnection.Connect(toGet.Host, toGet.Port);
			LdapEntry result = ldapConnection.Read(toGet.getDN(), toGet.AttributeArray);
			ldapConnection.Disconnect();
			return result;
		}

		public static LdapEntry Read(LdapUrl toGet, LdapSearchConstraints cons)
		{
			LdapConnection ldapConnection = new LdapConnection();
			ldapConnection.Connect(toGet.Host, toGet.Port);
			LdapEntry result = ldapConnection.Read(toGet.getDN(), toGet.AttributeArray, cons);
			ldapConnection.Disconnect();
			return result;
		}

		public virtual void Rename(string dn, string newRdn, bool deleteOldRdn)
		{
			Rename(dn, newRdn, deleteOldRdn, defSearchCons);
		}

		public virtual void Rename(string dn, string newRdn, bool deleteOldRdn, LdapConstraints cons)
		{
			Rename(dn, newRdn, null, deleteOldRdn, cons);
		}

		public virtual void Rename(string dn, string newRdn, string newParentdn, bool deleteOldRdn)
		{
			Rename(dn, newRdn, newParentdn, deleteOldRdn, defSearchCons);
		}

		public virtual void Rename(string dn, string newRdn, string newParentdn, bool deleteOldRdn, LdapConstraints cons)
		{
			LdapResponseQueue ldapResponseQueue = Rename(dn, newRdn, newParentdn, deleteOldRdn, null, cons);
			LdapResponse ldapResponse = (LdapResponse)ldapResponseQueue.getResponse();
			lock (responseCtlSemaphore)
			{
				responseCtls = ldapResponse.Controls;
			}
			chkResultCode(ldapResponseQueue, cons, ldapResponse);
		}

		public virtual LdapResponseQueue Rename(string dn, string newRdn, bool deleteOldRdn, LdapResponseQueue queue)
		{
			return Rename(dn, newRdn, deleteOldRdn, queue, defSearchCons);
		}

		public virtual LdapResponseQueue Rename(string dn, string newRdn, bool deleteOldRdn, LdapResponseQueue queue, LdapConstraints cons)
		{
			return Rename(dn, newRdn, null, deleteOldRdn, queue, cons);
		}

		public virtual LdapResponseQueue Rename(string dn, string newRdn, string newParentdn, bool deleteOldRdn, LdapResponseQueue queue)
		{
			return Rename(dn, newRdn, newParentdn, deleteOldRdn, queue, defSearchCons);
		}

		public virtual LdapResponseQueue Rename(string dn, string newRdn, string newParentdn, bool deleteOldRdn, LdapResponseQueue queue, LdapConstraints cons)
		{
			if (dn == null || newRdn == null)
			{
				throw new ArgumentException("RDN_PARAM_ERROR");
			}
			if (cons == null)
			{
				cons = defSearchCons;
			}
			LdapMessage msg = new LdapModifyDNRequest(dn, newRdn, newParentdn, deleteOldRdn, cons.getControls());
			return SendRequestToServer(msg, cons.TimeLimit, queue, null);
		}

		public virtual LdapSearchResults Search(string base_Renamed, int scope, string filter, string[] attrs, bool typesOnly)
		{
			return Search(base_Renamed, scope, filter, attrs, typesOnly, defSearchCons);
		}

		public virtual LdapSearchResults Search(string base_Renamed, int scope, string filter, string[] attrs, bool typesOnly, LdapSearchConstraints cons)
		{
			LdapSearchQueue queue = Search(base_Renamed, scope, filter, attrs, typesOnly, null, cons);
			if (cons == null)
			{
				cons = defSearchCons;
			}
			return new LdapSearchResults(this, queue, cons);
		}

		public virtual LdapSearchQueue Search(string base_Renamed, int scope, string filter, string[] attrs, bool typesOnly, LdapSearchQueue queue)
		{
			return Search(base_Renamed, scope, filter, attrs, typesOnly, queue, defSearchCons);
		}

		public virtual LdapSearchQueue Search(string base_Renamed, int scope, string filter, string[] attrs, bool typesOnly, LdapSearchQueue queue, LdapSearchConstraints cons)
		{
			if (filter == null)
			{
				filter = "objectclass=*";
			}
			if (cons == null)
			{
				cons = defSearchCons;
			}
			LdapMessage msg = new LdapSearchRequest(base_Renamed, scope, filter, attrs, cons.Dereference, cons.MaxResults, cons.ServerTimeLimit, typesOnly, cons.getControls());
			LdapSearchQueue ldapSearchQueue = queue;
			MessageAgent messageAgent;
			if (ldapSearchQueue == null)
			{
				messageAgent = new MessageAgent();
				ldapSearchQueue = new LdapSearchQueue(messageAgent);
			}
			else
			{
				messageAgent = queue.MessageAgent;
			}
			try
			{
				messageAgent.sendMessage(conn, msg, cons.TimeLimit, ldapSearchQueue, null);
				return ldapSearchQueue;
			}
			catch (LdapException ex)
			{
				throw ex;
			}
		}

		public static LdapSearchResults Search(LdapUrl toGet)
		{
			return Search(toGet, null);
		}

		public static LdapSearchResults Search(LdapUrl toGet, LdapSearchConstraints cons)
		{
			LdapConnection ldapConnection = new LdapConnection();
			ldapConnection.Connect(toGet.Host, toGet.Port);
			cons = ((cons != null) ? ((LdapSearchConstraints)cons.Clone()) : ldapConnection.SearchConstraints);
			cons.BatchSize = 0;
			LdapSearchResults result = ldapConnection.Search(toGet.getDN(), toGet.Scope, toGet.Filter, toGet.AttributeArray, typesOnly: false, cons);
			ldapConnection.Disconnect();
			return result;
		}

		public virtual LdapMessageQueue SendRequest(LdapMessage request, LdapMessageQueue queue)
		{
			return SendRequest(request, queue, null);
		}

		public virtual LdapMessageQueue SendRequest(LdapMessage request, LdapMessageQueue queue, LdapConstraints cons)
		{
			if (!request.Request)
			{
				throw new SystemException("Object is not a request message");
			}
			if (cons == null)
			{
				cons = defSearchCons;
			}
			LdapMessageQueue ldapMessageQueue = queue;
			MessageAgent messageAgent;
			if (ldapMessageQueue != null)
			{
				messageAgent = ((request.Type != 3) ? queue.MessageAgent : queue.MessageAgent);
			}
			else
			{
				messageAgent = new MessageAgent();
				ldapMessageQueue = ((request.Type != 3) ? ((LdapMessageQueue)new LdapResponseQueue(messageAgent)) : ((LdapMessageQueue)new LdapSearchQueue(messageAgent)));
			}
			try
			{
				messageAgent.sendMessage(conn, request, cons.TimeLimit, ldapMessageQueue, null);
				return ldapMessageQueue;
			}
			catch (LdapException ex)
			{
				throw ex;
			}
		}

		private LdapResponseQueue SendRequestToServer(LdapMessage msg, int timeout, LdapResponseQueue queue, BindProperties bindProps)
		{
			MessageAgent messageAgent;
			if (queue == null)
			{
				messageAgent = new MessageAgent();
				queue = new LdapResponseQueue(messageAgent);
			}
			else
			{
				messageAgent = queue.MessageAgent;
			}
			messageAgent.sendMessage(conn, msg, timeout, queue, bindProps);
			return queue;
		}

		private ReferralInfo getReferralConnection(string[] referrals)
		{
			ReferralInfo referralInfo = null;
			Exception ex = null;
			LdapConnection ldapConnection = null;
			LdapReferralHandler referralHandler = defSearchCons.getReferralHandler();
			int num = 0;
			if (referralHandler == null || referralHandler is LdapAuthHandler)
			{
				for (num = 0; num < referrals.Length; num++)
				{
					string dn = null;
					sbyte[] passwd = null;
					try
					{
						ldapConnection = new LdapConnection();
						ldapConnection.Constraints = defSearchCons;
						LdapUrl ldapUrl = new LdapUrl(referrals[num]);
						ldapConnection.Connect(ldapUrl.Host, ldapUrl.Port);
						if (referralHandler != null && referralHandler is LdapAuthHandler)
						{
							LdapAuthProvider authProvider = ((LdapAuthHandler)referralHandler).getAuthProvider(ldapUrl.Host, ldapUrl.Port);
							dn = authProvider.DN;
							passwd = authProvider.Password;
						}
						ldapConnection.Bind(3, dn, passwd);
						ex = null;
						referralInfo = new ReferralInfo(ldapConnection, referrals, ldapUrl);
						ldapConnection.Connection.ActiveReferral = referralInfo;
					}
					catch (Exception ex2)
					{
						if (ldapConnection != null)
						{
							try
							{
								ldapConnection.Disconnect();
								ldapConnection = null;
								ex = ex2;
							}
							catch (LdapException)
							{
							}
						}
						continue;
					}
					break;
				}
			}
			else
			{
				try
				{
					ldapConnection = ((LdapBindHandler)referralHandler).Bind(referrals, this);
					if (ldapConnection == null)
					{
						LdapReferralException ex4 = new LdapReferralException("REFERRAL_ERROR");
						ex4.setReferrals(referrals);
						throw ex4;
					}
					for (int i = 0; i < referrals.Length; i++)
					{
						try
						{
							LdapUrl ldapUrl2 = new LdapUrl(referrals[i]);
							if (ldapUrl2.Host.ToUpper().Equals(ldapConnection.Host.ToUpper()) && ldapUrl2.Port == ldapConnection.Port)
							{
								referralInfo = new ReferralInfo(ldapConnection, referrals, ldapUrl2);
								break;
							}
						}
						catch (Exception)
						{
						}
					}
					if (referralInfo == null)
					{
						ex = new LdapLocalException("REFERRAL_BIND_MATCH", 91);
					}
				}
				catch (Exception ex6)
				{
					ldapConnection = null;
					ex = ex6;
				}
			}
			if (ex != null)
			{
				if (ex is LdapReferralException)
				{
					throw (LdapReferralException)ex;
				}
				LdapException rootException = ((!(ex is LdapException)) ? new LdapLocalException("SERVER_CONNECT_ERROR", new object[1] { conn.Host }, 91, ex) : ((LdapException)ex));
				LdapReferralException ex7 = new LdapReferralException("REFERRAL_ERROR", rootException);
				ex7.setReferrals(referrals);
				ex7.FailedReferral = referrals[referrals.Length - 1];
				throw ex7;
			}
			return referralInfo;
		}

		private void chkResultCode(LdapMessageQueue queue, LdapConstraints cons, LdapResponse response)
		{
			if (response.ResultCode == 10 && cons.ReferralFollowing)
			{
				ArrayList list = null;
				try
				{
					chaseReferral(queue, cons, response, response.Referrals, 0, searchReference: false, null);
				}
				finally
				{
					releaseReferralConnections(list);
				}
			}
			else
			{
				response.chkResultCode();
			}
		}

		internal virtual ArrayList chaseReferral(LdapMessageQueue queue, LdapConstraints cons, LdapMessage msg, string[] initialReferrals, int hopCount, bool searchReference, ArrayList connectionList)
		{
			ArrayList arrayList = connectionList;
			LdapConnection ldapConnection = null;
			ReferralInfo referralInfo = null;
			if (arrayList == null)
			{
				arrayList = new ArrayList(cons.HopLimit);
			}
			string[] array;
			LdapMessage requestingMessage;
			if (initialReferrals != null)
			{
				array = initialReferrals;
				requestingMessage = msg.RequestingMessage;
			}
			else
			{
				LdapResponse ldapResponse = (LdapResponse)queue.getResponse();
				if (ldapResponse.ResultCode != 10)
				{
					ldapResponse.chkResultCode();
					return arrayList;
				}
				array = ldapResponse.Referrals;
				requestingMessage = ldapResponse.RequestingMessage;
			}
			try
			{
				if (hopCount++ > cons.HopLimit)
				{
					throw new LdapLocalException("Max hops exceeded", 97);
				}
				referralInfo = getReferralConnection(array);
				ldapConnection = referralInfo.ReferralConnection;
				LdapUrl referralUrl = referralInfo.ReferralUrl;
				arrayList.Add(ldapConnection);
				LdapMessage msg2 = rebuildRequest(requestingMessage, referralUrl, searchReference);
				try
				{
					MessageAgent messageAgent = ((!(queue is LdapResponseQueue)) ? queue.MessageAgent : queue.MessageAgent);
					messageAgent.sendMessage(ldapConnection.Connection, msg2, defSearchCons.TimeLimit, queue, null);
				}
				catch (InterThreadException rootException)
				{
					LdapReferralException ex = new LdapReferralException("REFERRAL_SEND", 91, null, rootException);
					ex.setReferrals(initialReferrals);
					ReferralInfo activeReferral = ldapConnection.Connection.ActiveReferral;
					ex.FailedReferral = activeReferral.ReferralUrl.ToString();
					throw ex;
				}
				if (initialReferrals == null)
				{
					return chaseReferral(queue, cons, null, null, hopCount, searchReference: false, arrayList);
				}
				return arrayList;
			}
			catch (Exception ex2)
			{
				if (ex2 is LdapReferralException)
				{
					throw (LdapReferralException)ex2;
				}
				LdapReferralException ex3 = new LdapReferralException("REFERRAL_ERROR", ex2);
				ex3.setReferrals(array);
				if (referralInfo != null)
				{
					ex3.FailedReferral = referralInfo.ReferralUrl.ToString();
				}
				else
				{
					ex3.FailedReferral = array[array.Length - 1];
				}
				throw ex3;
			}
		}

		private LdapMessage rebuildRequest(LdapMessage msg, LdapUrl url, bool reference)
		{
			string dN = url.getDN();
			string filter = null;
			switch (msg.Type)
			{
			case 3:
				if (reference)
				{
					filter = url.Filter;
				}
				break;
			default:
				throw new LdapLocalException("IMPROPER_REFERRAL", new object[1] { msg.Type }, 82);
			case 0:
			case 6:
			case 8:
			case 10:
			case 12:
			case 14:
			case 23:
				break;
			}
			return msg.Clone(dN, filter, reference);
		}

		internal virtual void releaseReferralConnections(ArrayList list)
		{
			if (list == null)
			{
				return;
			}
			for (int num = list.Count - 1; num >= 0; num--)
			{
				try
				{
					LdapConnection obj = (LdapConnection)list[num];
					list.RemoveAt(num);
					obj.Disconnect();
				}
				catch (IndexOutOfRangeException)
				{
				}
				catch (LdapException)
				{
				}
			}
		}

		public virtual LdapSchema FetchSchema(string schemaDN)
		{
			return new LdapSchema(Read(schemaDN, LdapSchema.schemaTypeNames));
		}

		public virtual string GetSchemaDN()
		{
			return GetSchemaDN("");
		}

		public virtual string GetSchemaDN(string dn)
		{
			string[] array = new string[1] { "subschemaSubentry" };
			string[] stringValueArray = Read(dn, array).getAttribute(array[0]).StringValueArray;
			if (stringValueArray == null || stringValueArray.Length < 1)
			{
				throw new LdapLocalException("NO_SCHEMA", new object[1] { dn }, 94);
			}
			if (stringValueArray.Length > 1)
			{
				throw new LdapLocalException("MULTIPLE_SCHEMA", new object[1] { dn }, 19);
			}
			return stringValueArray[0];
		}
	}
}
