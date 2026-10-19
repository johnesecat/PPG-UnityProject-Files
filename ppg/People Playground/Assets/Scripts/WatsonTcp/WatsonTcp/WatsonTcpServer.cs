using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WatsonTcp
{
	public class WatsonTcpServer : global::System.IDisposable
	{
		private string _Header = "[WatsonTcpServer] ";

		private WatsonTcpServerSettings _Settings = new WatsonTcpServerSettings();

		private WatsonTcpServerEvents _Events = new WatsonTcpServerEvents();

		private WatsonTcpServerCallbacks _Callbacks = new WatsonTcpServerCallbacks();

		private WatsonTcpStatistics _Statistics = new WatsonTcpStatistics();

		private WatsonTcpKeepaliveSettings _Keepalive = new WatsonTcpKeepaliveSettings();

		private WatsonTcpServerSslConfiguration _SslConfiguration = new WatsonTcpServerSslConfiguration();

		private int _Connections;

		private bool _IsListening;

		private Mode _Mode;

		private TlsVersion _TlsVersion;

		private string _ListenerIp;

		private int _ListenerPort;

		private IPAddress _ListenerIpAddress;

		private TcpListener _Listener;

		private X509Certificate2 _SslCertificate;

		private ConcurrentDictionary<string, global::System.DateTime> _UnauthenticatedClients = new ConcurrentDictionary<string, global::System.DateTime>();

		private ConcurrentDictionary<string, ClientMetadata> _Clients = new ConcurrentDictionary<string, ClientMetadata>();

		private ConcurrentDictionary<string, global::System.DateTime> _ClientsLastSeen = new ConcurrentDictionary<string, global::System.DateTime>();

		private ConcurrentDictionary<string, global::System.DateTime> _ClientsKicked = new ConcurrentDictionary<string, global::System.DateTime>();

		private ConcurrentDictionary<string, global::System.DateTime> _ClientsTimedout = new ConcurrentDictionary<string, global::System.DateTime>();

		private CancellationTokenSource _TokenSource = new CancellationTokenSource();

		private CancellationToken _Token;

		private global::System.Threading.Tasks.Task _AcceptConnections;

		private global::System.Threading.Tasks.Task _MonitorClients;

		private readonly object _SyncResponseLock = new object();

		public WatsonTcpServerSettings Settings
		{
			get
			{
				return _Settings;
			}
			set
			{
				if (value == null)
				{
					_Settings = new WatsonTcpServerSettings();
				}
				else
				{
					_Settings = value;
				}
			}
		}

		public WatsonTcpServerEvents Events
		{
			get
			{
				return _Events;
			}
			set
			{
				if (value == null)
				{
					_Events = new WatsonTcpServerEvents();
				}
				else
				{
					_Events = value;
				}
			}
		}

		public WatsonTcpServerCallbacks Callbacks
		{
			get
			{
				return _Callbacks;
			}
			set
			{
				if (value == null)
				{
					_Callbacks = new WatsonTcpServerCallbacks();
				}
				else
				{
					_Callbacks = value;
				}
			}
		}

		public WatsonTcpStatistics Statistics => _Statistics;

		public WatsonTcpKeepaliveSettings Keepalive
		{
			get
			{
				return _Keepalive;
			}
			set
			{
				if (value == null)
				{
					_Keepalive = new WatsonTcpKeepaliveSettings();
				}
				else
				{
					_Keepalive = value;
				}
			}
		}

		public WatsonTcpServerSslConfiguration SslConfiguration
		{
			get
			{
				return _SslConfiguration;
			}
			set
			{
				if (value == null)
				{
					_SslConfiguration = new WatsonTcpServerSslConfiguration();
				}
				else
				{
					_SslConfiguration = value;
				}
			}
		}

		public int Connections => _Connections;

		public bool IsListening => _IsListening;

		private event EventHandler<SyncResponseReceivedEventArgs> _SyncResponseReceived
		{
			[CompilerGenerated]
			add
			{
				EventHandler<SyncResponseReceivedEventArgs> val = this._SyncResponseReceived;
				EventHandler<SyncResponseReceivedEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<SyncResponseReceivedEventArgs> val3 = (EventHandler<SyncResponseReceivedEventArgs>)(object)global::System.Delegate.Combine((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<SyncResponseReceivedEventArgs>>(ref this._SyncResponseReceived, val3, val2);
				}
				while (val != val2);
			}
			[CompilerGenerated]
			remove
			{
				EventHandler<SyncResponseReceivedEventArgs> val = this._SyncResponseReceived;
				EventHandler<SyncResponseReceivedEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<SyncResponseReceivedEventArgs> val3 = (EventHandler<SyncResponseReceivedEventArgs>)(object)global::System.Delegate.Remove((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<SyncResponseReceivedEventArgs>>(ref this._SyncResponseReceived, val3, val2);
				}
				while (val != val2);
			}
		}

		public WatsonTcpServer(string listenerIp, int listenerPort)
		{
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Expected O, but got Unknown
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			if (listenerPort < 1)
			{
				throw new ArgumentOutOfRangeException("listenerPort");
			}
			_Mode = Mode.Tcp;
			if (string.IsNullOrEmpty(listenerIp))
			{
				_ListenerIpAddress = IPAddress.Any;
				_ListenerIp = ((object)_ListenerIpAddress).ToString();
			}
			else if (listenerIp.Equals("localhost") || listenerIp.Equals("127.0.0.1") || listenerIp.Equals("::1"))
			{
				_ListenerIpAddress = IPAddress.Loopback;
				_ListenerIp = ((object)_ListenerIpAddress).ToString();
			}
			else
			{
				_ListenerIpAddress = IPAddress.Parse(listenerIp);
				_ListenerIp = listenerIp;
			}
			_ListenerPort = listenerPort;
			SerializationHelper.InstantiateConverter();
		}

		public WatsonTcpServer(string listenerIp, int listenerPort, string pfxCertFile, string pfxCertPass, TlsVersion tlsVersion = TlsVersion.Tls12)
		{
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Expected O, but got Unknown
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Expected O, but got Unknown
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Expected O, but got Unknown
			if (listenerPort < 1)
			{
				throw new ArgumentOutOfRangeException("listenerPort");
			}
			if (string.IsNullOrEmpty(pfxCertFile))
			{
				throw new ArgumentNullException("pfxCertFile");
			}
			_Mode = Mode.Ssl;
			_TlsVersion = tlsVersion;
			if (string.IsNullOrEmpty(listenerIp))
			{
				_ListenerIpAddress = IPAddress.Any;
				_ListenerIp = ((object)_ListenerIpAddress).ToString();
			}
			else if (listenerIp.Equals("localhost") || listenerIp.Equals("127.0.0.1") || listenerIp.Equals("::1"))
			{
				_ListenerIpAddress = IPAddress.Loopback;
				_ListenerIp = ((object)_ListenerIpAddress).ToString();
			}
			else
			{
				_ListenerIpAddress = IPAddress.Parse(listenerIp);
				_ListenerIp = listenerIp;
			}
			_SslCertificate = null;
			if (string.IsNullOrEmpty(pfxCertPass))
			{
				_SslCertificate = new X509Certificate2(pfxCertFile);
			}
			else
			{
				_SslCertificate = new X509Certificate2(pfxCertFile, pfxCertPass);
			}
			_ListenerPort = listenerPort;
			SerializationHelper.InstantiateConverter();
		}

		public WatsonTcpServer(string listenerIp, int listenerPort, X509Certificate2 cert, TlsVersion tlsVersion = TlsVersion.Tls12)
		{
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Expected O, but got Unknown
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			if (listenerPort < 1)
			{
				throw new ArgumentOutOfRangeException("listenerPort");
			}
			if (cert == null)
			{
				throw new ArgumentNullException("cert");
			}
			_Mode = Mode.Ssl;
			_TlsVersion = tlsVersion;
			_SslCertificate = cert;
			if (string.IsNullOrEmpty(listenerIp))
			{
				_ListenerIpAddress = IPAddress.Any;
				_ListenerIp = ((object)_ListenerIpAddress).ToString();
			}
			else if (listenerIp.Equals("localhost") || listenerIp.Equals("127.0.0.1") || listenerIp.Equals("::1"))
			{
				_ListenerIpAddress = IPAddress.Loopback;
				_ListenerIp = ((object)_ListenerIpAddress).ToString();
			}
			else
			{
				_ListenerIpAddress = IPAddress.Parse(listenerIp);
				_ListenerIp = listenerIp;
			}
			_ListenerPort = listenerPort;
			SerializationHelper.InstantiateConverter();
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize((object)this);
		}

		public void Start()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Expected O, but got Unknown
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Expected O, but got Unknown
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			if (_IsListening)
			{
				throw new InvalidOperationException("WatsonTcpServer is already running.");
			}
			if (_UnauthenticatedClients == null)
			{
				_UnauthenticatedClients = new ConcurrentDictionary<string, global::System.DateTime>();
			}
			if (_Clients == null)
			{
				_Clients = new ConcurrentDictionary<string, ClientMetadata>();
			}
			if (_ClientsLastSeen == null)
			{
				_ClientsLastSeen = new ConcurrentDictionary<string, global::System.DateTime>();
			}
			if (_ClientsKicked == null)
			{
				_ClientsKicked = new ConcurrentDictionary<string, global::System.DateTime>();
			}
			if (_ClientsTimedout == null)
			{
				_ClientsTimedout = new ConcurrentDictionary<string, global::System.DateTime>();
			}
			_TokenSource = new CancellationTokenSource();
			_Token = _TokenSource.get_Token();
			_Statistics = new WatsonTcpStatistics();
			_Listener = new TcpListener(_ListenerIpAddress, _ListenerPort);
			if (!_Events.IsUsingMessages && !_Events.IsUsingStreams)
			{
				throw new InvalidOperationException("One of either 'MessageReceived' or 'StreamReceived' events must first be set.");
			}
			if (_Mode == Mode.Tcp)
			{
				_Settings.Logger?.Invoke(Severity.Info, string.Concat(new string[5]
				{
					_Header,
					"starting on ",
					_ListenerIp,
					":",
					_ListenerPort.ToString()
				}));
			}
			else
			{
				if (_Mode != Mode.Ssl)
				{
					throw new ArgumentException("Unknown mode: " + ((object)_Mode).ToString());
				}
				_Settings.Logger?.Invoke(Severity.Info, string.Concat(new string[5]
				{
					_Header,
					"starting with SSL on ",
					_ListenerIp,
					":",
					_ListenerPort.ToString()
				}));
			}
			_Listener.Start();
			_AcceptConnections = global::System.Threading.Tasks.Task.Run((Func<global::System.Threading.Tasks.Task>)(() => AcceptConnections()), _Token);
			_MonitorClients = global::System.Threading.Tasks.Task.Run((Func<global::System.Threading.Tasks.Task>)(() => MonitorForIdleClients()), _Token);
			_Events.HandleServerStarted(this, EventArgs.Empty);
		}

		public void Stop()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			if (!_IsListening)
			{
				throw new InvalidOperationException("WatsonTcpServer is not running.");
			}
			try
			{
				_IsListening = false;
				_Listener.Stop();
				_TokenSource.Cancel();
				_Settings.Logger?.Invoke(Severity.Info, _Header + "stopped");
				_Events.HandleServerStopped(this, EventArgs.Empty);
			}
			catch (global::System.Exception e)
			{
				_Events.HandleExceptionEncountered(this, new ExceptionEventArgs(e));
				throw;
			}
		}

		public bool Send(string ipPort, string data, Dictionary<object, object> metadata = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			if (string.IsNullOrEmpty(ipPort))
			{
				throw new ArgumentNullException("ipPort");
			}
			byte[] data2 = new byte[0];
			if (!string.IsNullOrEmpty(data))
			{
				data2 = Encoding.get_UTF8().GetBytes(data);
			}
			return Send(ipPort, data2, metadata);
		}

		public bool Send(string ipPort, byte[] data, Dictionary<object, object> metadata = null, int start = 0)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			if (string.IsNullOrEmpty(ipPort))
			{
				throw new ArgumentNullException("ipPort");
			}
			ClientMetadata clientMetadata = default(ClientMetadata);
			if (!_Clients.TryGetValue(ipPort, ref clientMetadata))
			{
				_Settings.Logger?.Invoke(Severity.Error, _Header + "unable to find client " + ipPort);
				return false;
			}
			if (data == null)
			{
				data = new byte[0];
			}
			WatsonCommon.BytesToStream(data, start, out var contentLength, out var stream);
			return Send(ipPort, contentLength, stream, metadata);
		}

		public bool Send(string ipPort, long contentLength, Stream stream, Dictionary<object, object> metadata = null)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Expected O, but got Unknown
			if (contentLength < 0)
			{
				throw new ArgumentException("Content length must be zero or greater.");
			}
			if (string.IsNullOrEmpty(ipPort))
			{
				throw new ArgumentNullException("ipPort");
			}
			ClientMetadata client = default(ClientMetadata);
			if (!_Clients.TryGetValue(ipPort, ref client))
			{
				_Settings.Logger?.Invoke(Severity.Error, _Header + "unable to find client " + ipPort);
				return false;
			}
			if (stream == null)
			{
				stream = (Stream)new MemoryStream(new byte[0]);
			}
			WatsonMessage msg = new WatsonMessage(metadata, contentLength, stream, syncRequest: false, syncResponse: false, null, null, _Settings.DebugMessages ? _Settings.Logger : null);
			return SendInternal(client, msg, contentLength, stream);
		}

		[AsyncStateMachine(typeof(<SendAsync>d__59))]
		public global::System.Threading.Tasks.Task<bool> SendAsync(string ipPort, string data, Dictionary<object, object> metadata = null, int start = 0, CancellationToken token = default(CancellationToken))
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			<SendAsync>d__59 <SendAsync>d__ = default(<SendAsync>d__59);
			<SendAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<SendAsync>d__.<>4__this = this;
			<SendAsync>d__.ipPort = ipPort;
			<SendAsync>d__.data = data;
			<SendAsync>d__.metadata = metadata;
			<SendAsync>d__.start = start;
			<SendAsync>d__.token = token;
			<SendAsync>d__.<>1__state = -1;
			<SendAsync>d__.<>t__builder.Start<<SendAsync>d__59>(ref <SendAsync>d__);
			return <SendAsync>d__.<>t__builder.get_Task();
		}

		[AsyncStateMachine(typeof(<SendAsync>d__60))]
		public global::System.Threading.Tasks.Task<bool> SendAsync(string ipPort, byte[] data, Dictionary<object, object> metadata = null, int start = 0, CancellationToken token = default(CancellationToken))
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			<SendAsync>d__60 <SendAsync>d__ = default(<SendAsync>d__60);
			<SendAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<SendAsync>d__.<>4__this = this;
			<SendAsync>d__.ipPort = ipPort;
			<SendAsync>d__.data = data;
			<SendAsync>d__.metadata = metadata;
			<SendAsync>d__.start = start;
			<SendAsync>d__.token = token;
			<SendAsync>d__.<>1__state = -1;
			<SendAsync>d__.<>t__builder.Start<<SendAsync>d__60>(ref <SendAsync>d__);
			return <SendAsync>d__.<>t__builder.get_Task();
		}

		[AsyncStateMachine(typeof(<SendAsync>d__61))]
		public global::System.Threading.Tasks.Task<bool> SendAsync(string ipPort, long contentLength, Stream stream, Dictionary<object, object> metadata = null, CancellationToken token = default(CancellationToken))
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			<SendAsync>d__61 <SendAsync>d__ = default(<SendAsync>d__61);
			<SendAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<SendAsync>d__.<>4__this = this;
			<SendAsync>d__.ipPort = ipPort;
			<SendAsync>d__.contentLength = contentLength;
			<SendAsync>d__.stream = stream;
			<SendAsync>d__.metadata = metadata;
			<SendAsync>d__.token = token;
			<SendAsync>d__.<>1__state = -1;
			<SendAsync>d__.<>t__builder.Start<<SendAsync>d__61>(ref <SendAsync>d__);
			return <SendAsync>d__.<>t__builder.get_Task();
		}

		public SyncResponse SendAndWait(int timeoutMs, string ipPort, string data, Dictionary<object, object> metadata = null)
		{
			byte[] data2 = new byte[0];
			if (!string.IsNullOrEmpty(data))
			{
				data2 = Encoding.get_UTF8().GetBytes(data);
			}
			return SendAndWait(timeoutMs, ipPort, data2, metadata);
		}

		public SyncResponse SendAndWait(int timeoutMs, string ipPort, byte[] data, Dictionary<object, object> metadata = null, int start = 0)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			if (string.IsNullOrEmpty(ipPort))
			{
				throw new ArgumentNullException("ipPort");
			}
			if (timeoutMs < 1000)
			{
				throw new ArgumentException("Timeout milliseconds must be 1000 or greater.");
			}
			ClientMetadata clientMetadata = default(ClientMetadata);
			if (!_Clients.TryGetValue(ipPort, ref clientMetadata))
			{
				_Settings.Logger?.Invoke(Severity.Error, _Header + "unable to find client " + ipPort);
				throw new KeyNotFoundException("Unable to find client " + ipPort + ".");
			}
			if (data == null)
			{
				data = new byte[0];
			}
			WatsonCommon.BytesToStream(data, start, out var contentLength, out var stream);
			return SendAndWait(timeoutMs, ipPort, contentLength, stream, metadata);
		}

		public SyncResponse SendAndWait(int timeoutMs, string ipPort, long contentLength, Stream stream, Dictionary<object, object> metadata = null)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Expected O, but got Unknown
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			if (contentLength < 0)
			{
				throw new ArgumentException("Content length must be zero or greater.");
			}
			if (string.IsNullOrEmpty(ipPort))
			{
				throw new ArgumentNullException("ipPort");
			}
			if (timeoutMs < 1000)
			{
				throw new ArgumentException("Timeout milliseconds must be 1000 or greater.");
			}
			ClientMetadata client = default(ClientMetadata);
			if (!_Clients.TryGetValue(ipPort, ref client))
			{
				_Settings.Logger?.Invoke(Severity.Error, _Header + "unable to find client " + ipPort);
				throw new KeyNotFoundException("Unable to find client " + ipPort + ".");
			}
			if (stream == null)
			{
				stream = (Stream)new MemoryStream(new byte[0]);
			}
			global::System.DateTime dateTime = global::System.DateTime.get_Now().AddMilliseconds((double)timeoutMs);
			Stream stream2 = stream;
			global::System.DateTime? expiration = dateTime;
			Guid val = Guid.NewGuid();
			WatsonMessage msg = new WatsonMessage(metadata, contentLength, stream2, syncRequest: true, syncResponse: false, expiration, ((object)(Guid)(ref val)).ToString(), _Settings.DebugMessages ? _Settings.Logger : null);
			return SendAndWaitInternal(client, msg, timeoutMs, contentLength, stream);
		}

		public bool IsClientConnected(string ipPort)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			if (string.IsNullOrEmpty(ipPort))
			{
				throw new ArgumentNullException("ipPort");
			}
			ClientMetadata clientMetadata = default(ClientMetadata);
			return _Clients.TryGetValue(ipPort, ref clientMetadata);
		}

		public global::System.Collections.Generic.IEnumerable<string> ListClients()
		{
			return (global::System.Collections.Generic.IEnumerable<string>)Enumerable.ToList<string>((global::System.Collections.Generic.IEnumerable<string>)_Clients.get_Keys());
		}

		public void DisconnectClient(string ipPort, MessageStatus status = MessageStatus.Removed, bool sendNotice = true)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			if (string.IsNullOrEmpty(ipPort))
			{
				throw new ArgumentNullException("ipPort");
			}
			ClientMetadata clientMetadata = default(ClientMetadata);
			if (!_Clients.TryGetValue(ipPort, ref clientMetadata))
			{
				_Settings.Logger?.Invoke(Severity.Error, _Header + "unable to find client " + ipPort);
				return;
			}
			if (!_ClientsTimedout.ContainsKey(ipPort))
			{
				_ClientsKicked.TryAdd(ipPort, global::System.DateTime.get_Now());
			}
			if (sendNotice)
			{
				WatsonMessage watsonMessage = new WatsonMessage();
				watsonMessage.Status = status;
				SendInternal(clientMetadata, watsonMessage, 0L, null);
			}
			clientMetadata.Dispose();
			ClientMetadata clientMetadata2 = default(ClientMetadata);
			_Clients.TryRemove(ipPort, ref clientMetadata2);
		}

		public void DisconnectClients(MessageStatus status = MessageStatus.Removed, bool sendNotice = true)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			if (_Clients == null || _Clients.get_Count() <= 0)
			{
				return;
			}
			global::System.Collections.Generic.IEnumerator<KeyValuePair<string, ClientMetadata>> enumerator = _Clients.GetEnumerator();
			try
			{
				while (((global::System.Collections.IEnumerator)enumerator).MoveNext())
				{
					DisconnectClient(enumerator.get_Current().get_Value().IpPort, status, sendNotice);
				}
			}
			finally
			{
				((global::System.IDisposable)enumerator)?.Dispose();
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				_Settings.Logger?.Invoke(Severity.Info, _Header + "disposing");
				if (_IsListening)
				{
					Stop();
				}
				DisconnectClients(MessageStatus.Shutdown);
				if (_Listener != null && _Listener.get_Server() != null)
				{
					_Listener.get_Server().Close();
					_Listener.get_Server().Dispose();
				}
				_Settings = null;
				_Events = null;
				_Callbacks = null;
				_Statistics = null;
				_Keepalive = null;
				_SslConfiguration = null;
				_ListenerIp = null;
				_ListenerIpAddress = null;
				_Listener = null;
				_SslCertificate = null;
				_UnauthenticatedClients = null;
				_Clients = null;
				_ClientsLastSeen = null;
				_ClientsKicked = null;
				_ClientsTimedout = null;
				_TokenSource = null;
				_AcceptConnections = null;
				_MonitorClients = null;
				_IsListening = false;
			}
		}

		private void EnableKeepalives(TcpClient client)
		{
		}

		[AsyncStateMachine(typeof(<AcceptConnections>d__71))]
		private global::System.Threading.Tasks.Task AcceptConnections()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			<AcceptConnections>d__71 <AcceptConnections>d__ = default(<AcceptConnections>d__71);
			<AcceptConnections>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AcceptConnections>d__.<>4__this = this;
			<AcceptConnections>d__.<>1__state = -1;
			((AsyncTaskMethodBuilder)(ref <AcceptConnections>d__.<>t__builder)).Start<<AcceptConnections>d__71>(ref <AcceptConnections>d__);
			return ((AsyncTaskMethodBuilder)(ref <AcceptConnections>d__.<>t__builder)).get_Task();
		}

		[AsyncStateMachine(typeof(<StartTls>d__72))]
		private global::System.Threading.Tasks.Task<bool> StartTls(ClientMetadata client)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			<StartTls>d__72 <StartTls>d__ = default(<StartTls>d__72);
			<StartTls>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<StartTls>d__.<>4__this = this;
			<StartTls>d__.client = client;
			<StartTls>d__.<>1__state = -1;
			<StartTls>d__.<>t__builder.Start<<StartTls>d__72>(ref <StartTls>d__);
			return <StartTls>d__.<>t__builder.get_Task();
		}

		private void FinalizeConnection(ClientMetadata client, CancellationToken token)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			<>c__DisplayClass73_0 CS$<>8__locals0 = new <>c__DisplayClass73_0();
			CS$<>8__locals0.<>4__this = this;
			CS$<>8__locals0.client = client;
			CS$<>8__locals0.token = token;
			_Clients.TryAdd(CS$<>8__locals0.client.IpPort, CS$<>8__locals0.client);
			_ClientsLastSeen.TryAdd(CS$<>8__locals0.client.IpPort, global::System.DateTime.get_Now());
			if (!string.IsNullOrEmpty(_Settings.PresharedKey))
			{
				_Settings.Logger?.Invoke(Severity.Debug, _Header + "requesting authentication material from " + CS$<>8__locals0.client.IpPort);
				_UnauthenticatedClients.TryAdd(CS$<>8__locals0.client.IpPort, global::System.DateTime.get_Now());
				Encoding.get_UTF8().GetBytes("Authentication required");
				WatsonMessage watsonMessage = new WatsonMessage();
				watsonMessage.Status = MessageStatus.AuthRequired;
				SendInternal(CS$<>8__locals0.client, watsonMessage, 0L, null);
			}
			_Settings.Logger?.Invoke(Severity.Debug, _Header + "starting data receiver for " + CS$<>8__locals0.client.IpPort);
			CS$<>8__locals0.client.DataReceiver = global::System.Threading.Tasks.Task.Run((Func<global::System.Threading.Tasks.Task>)(() => CS$<>8__locals0.<>4__this.DataReceiver(CS$<>8__locals0.client, CS$<>8__locals0.token)), CS$<>8__locals0.token);
			_Events.HandleClientConnected(this, new ConnectionEventArgs(CS$<>8__locals0.client.IpPort));
		}

		private bool IsConnected(ClientMetadata client)
		{
			if (client != null && client.TcpClient != null)
			{
				if (client.TcpClient.get_Connected())
				{
					byte[] array = new byte[1];
					bool flag = false;
					try
					{
						client.WriteLock.Wait();
						client.TcpClient.get_Client().Send(array, 0, (SocketFlags)0);
						flag = true;
					}
					catch (SocketException val)
					{
						if (((Win32Exception)val).get_NativeErrorCode().Equals(10035))
						{
							flag = true;
						}
					}
					catch (global::System.Exception)
					{
					}
					finally
					{
						if (client != null)
						{
							client.WriteLock.Release();
						}
					}
					if (flag)
					{
						return true;
					}
					try
					{
						client.WriteLock.Wait();
						if (client.TcpClient.get_Client().Poll(0, (SelectMode)1) && !client.TcpClient.get_Client().Poll(0, (SelectMode)2))
						{
							byte[] array2 = new byte[1];
							if (client.TcpClient.get_Client().Receive(array2, (SocketFlags)2) == 0)
							{
								return false;
							}
							return true;
						}
						return false;
					}
					catch (global::System.Exception)
					{
						return false;
					}
					finally
					{
						if (client != null)
						{
							client.WriteLock.Release();
						}
					}
				}
				return false;
			}
			return false;
		}

		[AsyncStateMachine(typeof(<DataReceiver>d__75))]
		private global::System.Threading.Tasks.Task DataReceiver(ClientMetadata client, CancellationToken token)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			<DataReceiver>d__75 <DataReceiver>d__ = default(<DataReceiver>d__75);
			<DataReceiver>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DataReceiver>d__.<>4__this = this;
			<DataReceiver>d__.client = client;
			<DataReceiver>d__.token = token;
			<DataReceiver>d__.<>1__state = -1;
			((AsyncTaskMethodBuilder)(ref <DataReceiver>d__.<>t__builder)).Start<<DataReceiver>d__75>(ref <DataReceiver>d__);
			return ((AsyncTaskMethodBuilder)(ref <DataReceiver>d__.<>t__builder)).get_Task();
		}

		private bool SendInternal(ClientMetadata client, WatsonMessage msg, long contentLength, Stream stream)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			if (client == null)
			{
				throw new ArgumentNullException("client");
			}
			if (msg == null)
			{
				throw new ArgumentNullException("msg");
			}
			if (contentLength > 0 && (stream == null || !stream.get_CanRead()))
			{
				throw new ArgumentException("Cannot read from supplied stream.");
			}
			client.WriteLock.Wait();
			try
			{
				SendHeaders(client, msg);
				SendDataStream(client, contentLength, stream);
				_Statistics.IncrementSentMessages();
				_Statistics.AddSentBytes(contentLength);
				return true;
			}
			catch (TaskCanceledException)
			{
				return false;
			}
			catch (OperationCanceledException)
			{
				return false;
			}
			catch (global::System.Exception ex)
			{
				_Settings.Logger?.Invoke(Severity.Error, string.Concat(new string[6]
				{
					_Header,
					"failed to write message to ",
					client.IpPort,
					": ",
					Environment.get_NewLine(),
					SerializationHelper.SerializeJson(ex, pretty: true)
				}));
				_Events.HandleExceptionEncountered(this, new ExceptionEventArgs(ex));
				return false;
			}
			finally
			{
				if (client != null)
				{
					client.WriteLock.Release();
				}
			}
		}

		[AsyncStateMachine(typeof(<SendInternalAsync>d__77))]
		private global::System.Threading.Tasks.Task<bool> SendInternalAsync(ClientMetadata client, WatsonMessage msg, long contentLength, Stream stream, CancellationToken token)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			<SendInternalAsync>d__77 <SendInternalAsync>d__ = default(<SendInternalAsync>d__77);
			<SendInternalAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<SendInternalAsync>d__.<>4__this = this;
			<SendInternalAsync>d__.client = client;
			<SendInternalAsync>d__.msg = msg;
			<SendInternalAsync>d__.contentLength = contentLength;
			<SendInternalAsync>d__.stream = stream;
			<SendInternalAsync>d__.token = token;
			<SendInternalAsync>d__.<>1__state = -1;
			<SendInternalAsync>d__.<>t__builder.Start<<SendInternalAsync>d__77>(ref <SendInternalAsync>d__);
			return <SendInternalAsync>d__.<>t__builder.get_Task();
		}

		private SyncResponse SendAndWaitInternal(ClientMetadata client, WatsonMessage msg, int timeoutMs, long contentLength, Stream stream)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Expected O, but got Unknown
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			<>c__DisplayClass78_0 CS$<>8__locals0 = new <>c__DisplayClass78_0();
			CS$<>8__locals0.msg = msg;
			if (client == null)
			{
				throw new ArgumentNullException("client");
			}
			if (CS$<>8__locals0.msg == null)
			{
				throw new ArgumentNullException("msg");
			}
			if (contentLength > 0 && (stream == null || !stream.get_CanRead()))
			{
				throw new ArgumentException("Cannot read from supplied stream.");
			}
			client.WriteLock.Wait();
			CS$<>8__locals0.ret = null;
			CS$<>8__locals0.Responded = new AutoResetEvent(false);
			EventHandler<SyncResponseReceivedEventArgs> value = delegate(object sender, SyncResponseReceivedEventArgs e)
			{
				if (e.Message.ConversationGuid == CS$<>8__locals0.msg.ConversationGuid)
				{
					CS$<>8__locals0.ret = new SyncResponse(e.Message.Expiration.get_Value(), e.Message.Metadata, e.Data);
					((EventWaitHandle)CS$<>8__locals0.Responded).Set();
				}
			};
			_SyncResponseReceived += value;
			try
			{
				SendHeaders(client, CS$<>8__locals0.msg);
				SendDataStream(client, contentLength, stream);
				_Statistics.IncrementSentMessages();
				_Statistics.AddSentBytes(contentLength);
			}
			catch (global::System.Exception ex)
			{
				_Settings.Logger?.Invoke(Severity.Error, string.Concat(new string[6]
				{
					_Header,
					"failed to write message to ",
					client.IpPort,
					" due to exception: ",
					Environment.get_NewLine(),
					SerializationHelper.SerializeJson(ex, pretty: true)
				}));
				_Events.HandleExceptionEncountered(this, new ExceptionEventArgs(ex));
				throw;
			}
			finally
			{
				if (client != null)
				{
					client.WriteLock.Release();
				}
			}
			((WaitHandle)CS$<>8__locals0.Responded).WaitOne(new TimeSpan(0, 0, 0, 0, timeoutMs));
			_SyncResponseReceived -= value;
			if (CS$<>8__locals0.ret != null)
			{
				return CS$<>8__locals0.ret;
			}
			_Settings.Logger?.Invoke(Severity.Error, _Header + "synchronous response not received within the timeout window");
			throw new TimeoutException("A response to a synchronous request was not received within the timeout window.");
		}

		private void SendHeaders(ClientMetadata client, WatsonMessage msg)
		{
			byte[] headerBytes = msg.HeaderBytes;
			client.DataStream.Write(headerBytes, 0, headerBytes.Length);
			client.DataStream.Flush();
		}

		[AsyncStateMachine(typeof(<SendHeadersAsync>d__80))]
		private global::System.Threading.Tasks.Task SendHeadersAsync(ClientMetadata client, WatsonMessage msg, CancellationToken token)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			<SendHeadersAsync>d__80 <SendHeadersAsync>d__ = default(<SendHeadersAsync>d__80);
			<SendHeadersAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SendHeadersAsync>d__.client = client;
			<SendHeadersAsync>d__.msg = msg;
			<SendHeadersAsync>d__.token = token;
			<SendHeadersAsync>d__.<>1__state = -1;
			((AsyncTaskMethodBuilder)(ref <SendHeadersAsync>d__.<>t__builder)).Start<<SendHeadersAsync>d__80>(ref <SendHeadersAsync>d__);
			return ((AsyncTaskMethodBuilder)(ref <SendHeadersAsync>d__.<>t__builder)).get_Task();
		}

		private void SendDataStream(ClientMetadata client, long contentLength, Stream stream)
		{
			if (contentLength <= 0)
			{
				return;
			}
			long num = contentLength;
			int num2 = 0;
			if (_Settings.StreamBufferSize != client.SendBuffer.Length)
			{
				client.SendBuffer = new byte[_Settings.StreamBufferSize];
			}
			while (num > 0)
			{
				num2 = stream.Read(client.SendBuffer, 0, client.SendBuffer.Length);
				if (num2 > 0)
				{
					client.DataStream.Write(client.SendBuffer, 0, num2);
					num -= num2;
				}
			}
			client.DataStream.Flush();
		}

		[AsyncStateMachine(typeof(<SendDataStreamAsync>d__82))]
		private global::System.Threading.Tasks.Task SendDataStreamAsync(ClientMetadata client, long contentLength, Stream stream, CancellationToken token)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			<SendDataStreamAsync>d__82 <SendDataStreamAsync>d__ = default(<SendDataStreamAsync>d__82);
			<SendDataStreamAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SendDataStreamAsync>d__.<>4__this = this;
			<SendDataStreamAsync>d__.client = client;
			<SendDataStreamAsync>d__.contentLength = contentLength;
			<SendDataStreamAsync>d__.stream = stream;
			<SendDataStreamAsync>d__.token = token;
			<SendDataStreamAsync>d__.<>1__state = -1;
			((AsyncTaskMethodBuilder)(ref <SendDataStreamAsync>d__.<>t__builder)).Start<<SendDataStreamAsync>d__82>(ref <SendDataStreamAsync>d__);
			return ((AsyncTaskMethodBuilder)(ref <SendDataStreamAsync>d__.<>t__builder)).get_Task();
		}

		[AsyncStateMachine(typeof(<MonitorForIdleClients>d__83))]
		private global::System.Threading.Tasks.Task MonitorForIdleClients()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			<MonitorForIdleClients>d__83 <MonitorForIdleClients>d__ = default(<MonitorForIdleClients>d__83);
			<MonitorForIdleClients>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<MonitorForIdleClients>d__.<>4__this = this;
			<MonitorForIdleClients>d__.<>1__state = -1;
			((AsyncTaskMethodBuilder)(ref <MonitorForIdleClients>d__.<>t__builder)).Start<<MonitorForIdleClients>d__83>(ref <MonitorForIdleClients>d__);
			return ((AsyncTaskMethodBuilder)(ref <MonitorForIdleClients>d__.<>t__builder)).get_Task();
		}
	}
}
