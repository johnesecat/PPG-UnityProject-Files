using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WatsonTcp
{
	public class WatsonTcpClient : global::System.IDisposable
	{
		private string _Header = "[WatsonTcpClient] ";

		private WatsonTcpClientSettings _Settings = new WatsonTcpClientSettings();

		private WatsonTcpClientEvents _Events = new WatsonTcpClientEvents();

		private WatsonTcpClientCallbacks _Callbacks = new WatsonTcpClientCallbacks();

		private WatsonTcpStatistics _Statistics = new WatsonTcpStatistics();

		private WatsonTcpKeepaliveSettings _Keepalive = new WatsonTcpKeepaliveSettings();

		private WatsonTcpClientSslConfiguration _SslConfiguration = new WatsonTcpClientSslConfiguration();

		private Mode _Mode;

		private TlsVersion _TlsVersion;

		private string _SourceIp;

		private int _SourcePort;

		private string _ServerIp;

		private int _ServerPort;

		private TcpClient _Client;

		private Stream _DataStream;

		private NetworkStream _TcpStream;

		private SslStream _SslStream;

		private X509Certificate2 _SslCertificate;

		private X509Certificate2Collection _SslCertificateCollection;

		private SemaphoreSlim _WriteLock = new SemaphoreSlim(1, 1);

		private SemaphoreSlim _ReadLock = new SemaphoreSlim(1, 1);

		private CancellationTokenSource _TokenSource = new CancellationTokenSource();

		private CancellationToken _Token;

		private global::System.Threading.Tasks.Task _DataReceiver;

		private global::System.Threading.Tasks.Task _IdleServerMonitor;

		private global::System.DateTime _LastActivity = global::System.DateTime.get_Now();

		private bool _IsTimeout;

		private byte[] _SendBuffer = new byte[65536];

		private readonly object _SyncResponseLock = new object();

		public WatsonTcpClientSettings Settings
		{
			get
			{
				return _Settings;
			}
			set
			{
				if (value == null)
				{
					_Settings = new WatsonTcpClientSettings();
				}
				else
				{
					_Settings = value;
				}
			}
		}

		public WatsonTcpClientEvents Events
		{
			get
			{
				return _Events;
			}
			set
			{
				if (value == null)
				{
					_Events = new WatsonTcpClientEvents();
				}
				else
				{
					_Events = value;
				}
			}
		}

		public WatsonTcpClientCallbacks Callbacks
		{
			get
			{
				return _Callbacks;
			}
			set
			{
				if (value == null)
				{
					_Callbacks = new WatsonTcpClientCallbacks();
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

		public WatsonTcpClientSslConfiguration SslConfiguration
		{
			get
			{
				return _SslConfiguration;
			}
			set
			{
				if (value == null)
				{
					_SslConfiguration = new WatsonTcpClientSslConfiguration();
				}
				else
				{
					_SslConfiguration = value;
				}
			}
		}

		public bool Connected
		{
			[CompilerGenerated]
			get
			{
				return <Connected>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<Connected>k__BackingField = value;
			}
		}

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

		public WatsonTcpClient(string serverIp, int serverPort)
		{
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Expected O, but got Unknown
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Expected O, but got Unknown
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Expected O, but got Unknown
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			if (string.IsNullOrEmpty(serverIp))
			{
				throw new ArgumentNullException("serverIp");
			}
			if (serverPort < 0)
			{
				throw new ArgumentOutOfRangeException("serverPort");
			}
			_Mode = Mode.Tcp;
			_ServerIp = serverIp;
			_ServerPort = serverPort;
			_SendBuffer = new byte[_Settings.StreamBufferSize];
			SerializationHelper.InstantiateConverter();
		}

		public WatsonTcpClient(string serverIp, int serverPort, string pfxCertFile, string pfxCertPass, TlsVersion tlsVersion = TlsVersion.Tls12)
		{
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Expected O, but got Unknown
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Expected O, but got Unknown
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Expected O, but got Unknown
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Expected O, but got Unknown
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Expected O, but got Unknown
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Expected O, but got Unknown
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Expected O, but got Unknown
			if (string.IsNullOrEmpty(serverIp))
			{
				throw new ArgumentNullException("serverIp");
			}
			if (serverPort < 0)
			{
				throw new ArgumentOutOfRangeException("serverPort");
			}
			_Mode = Mode.Ssl;
			_TlsVersion = tlsVersion;
			_ServerIp = serverIp;
			_ServerPort = serverPort;
			_SendBuffer = new byte[_Settings.StreamBufferSize];
			if (!string.IsNullOrEmpty(pfxCertFile))
			{
				if (string.IsNullOrEmpty(pfxCertPass))
				{
					_SslCertificate = new X509Certificate2(pfxCertFile);
				}
				else
				{
					_SslCertificate = new X509Certificate2(pfxCertFile, pfxCertPass);
				}
				X509Certificate2Collection val = new X509Certificate2Collection();
				val.Add(_SslCertificate);
				_SslCertificateCollection = val;
			}
			else
			{
				_SslCertificateCollection = new X509Certificate2Collection();
			}
			SerializationHelper.InstantiateConverter();
		}

		public WatsonTcpClient(string serverIp, int serverPort, X509Certificate2 cert, TlsVersion tlsVersion = TlsVersion.Tls12)
		{
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Expected O, but got Unknown
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Expected O, but got Unknown
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Expected O, but got Unknown
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Expected O, but got Unknown
			if (string.IsNullOrEmpty(serverIp))
			{
				throw new ArgumentNullException("serverIp");
			}
			if (serverPort < 0)
			{
				throw new ArgumentOutOfRangeException("serverPort");
			}
			if (cert == null)
			{
				throw new ArgumentNullException("cert");
			}
			_Mode = Mode.Ssl;
			_TlsVersion = tlsVersion;
			_SslCertificate = cert;
			_ServerIp = serverIp;
			_ServerPort = serverPort;
			_SendBuffer = new byte[_Settings.StreamBufferSize];
			X509Certificate2Collection val = new X509Certificate2Collection();
			val.Add(_SslCertificate);
			_SslCertificateCollection = val;
			SerializationHelper.InstantiateConverter();
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize((object)this);
		}

		public void Connect()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Expected O, but got Unknown
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Expected O, but got Unknown
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Expected O, but got Unknown
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Expected O, but got Unknown
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0418: Unknown result type (might be due to invalid IL or missing references)
			//IL_0457: Unknown result type (might be due to invalid IL or missing references)
			//IL_0461: Expected O, but got Unknown
			//IL_0470: Unknown result type (might be due to invalid IL or missing references)
			//IL_047a: Expected O, but got Unknown
			//IL_0492: Unknown result type (might be due to invalid IL or missing references)
			//IL_0516: Unknown result type (might be due to invalid IL or missing references)
			//IL_0588: Unknown result type (might be due to invalid IL or missing references)
			//IL_0607: Unknown result type (might be due to invalid IL or missing references)
			//IL_0688: Unknown result type (might be due to invalid IL or missing references)
			//IL_068f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0699: Expected O, but got Unknown
			//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
			if (Connected)
			{
				throw new InvalidOperationException("Already connected to the server.");
			}
			if (_Settings.LocalPort == 0)
			{
				_Client = new TcpClient();
			}
			else
			{
				IPEndPoint val = new IPEndPoint(IPAddress.Any, _Settings.LocalPort);
				_Client = new TcpClient(val);
			}
			_Client.set_NoDelay(_Settings.NoDelay);
			_Statistics = new WatsonTcpStatistics();
			IAsyncResult val2 = null;
			WaitHandle val3 = null;
			if (!_Events.IsUsingMessages && !_Events.IsUsingStreams)
			{
				throw new InvalidOperationException("One of either 'MessageReceived' or 'StreamReceived' events must first be set.");
			}
			if (_Mode == Mode.Tcp)
			{
				_Settings.Logger?.Invoke(Severity.Info, string.Concat(new string[5]
				{
					_Header,
					"connecting to ",
					_ServerIp,
					":",
					_ServerPort.ToString()
				}));
				_Client.set_LingerState(new LingerOption(true, 0));
				val2 = _Client.BeginConnect(_ServerIp, _ServerPort, (AsyncCallback)null, (object)null);
				val3 = val2.get_AsyncWaitHandle();
				try
				{
					if (!val3.WaitOne(TimeSpan.FromSeconds((double)_Settings.ConnectTimeoutSeconds), false))
					{
						_Client.Close();
						_Settings.Logger?.Invoke(Severity.Error, string.Concat(new string[5]
						{
							_Header,
							"timeout connecting to ",
							_ServerIp,
							":",
							_ServerPort.ToString()
						}));
						throw new TimeoutException("Timeout connecting to " + _ServerIp + ":" + _ServerPort);
					}
					_Client.EndConnect(val2);
					_SourceIp = ((object)((IPEndPoint)_Client.get_Client().get_LocalEndPoint()).get_Address()).ToString();
					_SourcePort = ((IPEndPoint)_Client.get_Client().get_LocalEndPoint()).get_Port();
					_TcpStream = _Client.GetStream();
					_DataStream = (Stream)(object)_TcpStream;
					_SslStream = null;
					if (_Keepalive.EnableTcpKeepAlives)
					{
						EnableKeepalives();
					}
					Connected = true;
				}
				catch (global::System.Exception ex)
				{
					_Settings.Logger?.Invoke(Severity.Error, _Header + "exception encountered: " + Environment.get_NewLine() + SerializationHelper.SerializeJson(ex, pretty: true));
					_Events.HandleExceptionEncountered(this, new ExceptionEventArgs(ex));
					throw;
				}
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
					"connecting with SSL to ",
					_ServerIp,
					":",
					_ServerPort.ToString()
				}));
				_Client.set_LingerState(new LingerOption(true, 0));
				val2 = _Client.BeginConnect(_ServerIp, _ServerPort, (AsyncCallback)null, (object)null);
				val3 = val2.get_AsyncWaitHandle();
				try
				{
					if (!val3.WaitOne(TimeSpan.FromSeconds((double)_Settings.ConnectTimeoutSeconds), false))
					{
						_Client.Close();
						_Settings.Logger?.Invoke(Severity.Error, string.Concat(new string[5]
						{
							_Header,
							"timeout connecting to ",
							_ServerIp,
							":",
							_ServerPort.ToString()
						}));
						throw new TimeoutException("Timeout connecting to " + _ServerIp + ":" + _ServerPort);
					}
					_Client.EndConnect(val2);
					_SourceIp = ((object)((IPEndPoint)_Client.get_Client().get_LocalEndPoint()).get_Address()).ToString();
					_SourcePort = ((IPEndPoint)_Client.get_Client().get_LocalEndPoint()).get_Port();
					if (_Settings.AcceptInvalidCertificates)
					{
						_SslStream = new SslStream((Stream)(object)_Client.GetStream(), false, _SslConfiguration.ServerCertificateValidationCallback, _SslConfiguration.ClientCertificateSelectionCallback);
					}
					else
					{
						_SslStream = new SslStream((Stream)(object)_Client.GetStream(), false);
					}
					_SslStream.AuthenticateAsClient(_ServerIp, (X509CertificateCollection)(object)_SslCertificateCollection, _TlsVersion.ToSslProtocols(), !_Settings.AcceptInvalidCertificates);
					if (!((AuthenticatedStream)_SslStream).get_IsEncrypted())
					{
						_Settings.Logger?.Invoke(Severity.Error, string.Concat(new string[6]
						{
							_Header,
							"stream to ",
							_ServerIp,
							":",
							_ServerPort.ToString(),
							" is not encrypted"
						}));
						throw new AuthenticationException("Stream is not encrypted");
					}
					if (!((AuthenticatedStream)_SslStream).get_IsAuthenticated())
					{
						_Settings.Logger?.Invoke(Severity.Error, string.Concat(new string[6]
						{
							_Header,
							"stream to ",
							_ServerIp,
							":",
							_ServerPort.ToString(),
							" is not authenticated"
						}));
						throw new AuthenticationException("Stream is not authenticated");
					}
					if (_Settings.MutuallyAuthenticate && !((AuthenticatedStream)_SslStream).get_IsMutuallyAuthenticated())
					{
						_Settings.Logger?.Invoke(Severity.Error, string.Concat(new string[6]
						{
							_Header,
							"mutual authentication with ",
							_ServerIp,
							":",
							_ServerPort.ToString(),
							" failed"
						}));
						throw new AuthenticationException("Mutual authentication failed");
					}
					_DataStream = (Stream)(object)_SslStream;
					Connected = true;
				}
				catch (global::System.Exception ex2)
				{
					_Settings.Logger?.Invoke(Severity.Error, _Header + "exception encountered: " + Environment.get_NewLine() + SerializationHelper.SerializeJson(ex2, pretty: true));
					_Events.HandleExceptionEncountered(this, new ExceptionEventArgs(ex2));
					throw;
				}
			}
			_TokenSource = new CancellationTokenSource();
			_Token = _TokenSource.get_Token();
			_LastActivity = global::System.DateTime.get_Now();
			_IsTimeout = false;
			_DataReceiver = global::System.Threading.Tasks.Task.Run((Func<global::System.Threading.Tasks.Task>)(() => DataReceiver()), _Token);
			_IdleServerMonitor = global::System.Threading.Tasks.Task.Run((Func<global::System.Threading.Tasks.Task>)(() => IdleServerMonitor()), _Token);
			_Events.HandleServerConnected(this, new ConnectionEventArgs(_ServerIp + ":" + _ServerPort));
			_Settings.Logger?.Invoke(Severity.Info, string.Concat(new string[5]
			{
				_Header,
				"connected to ",
				_ServerIp,
				":",
				_ServerPort.ToString()
			}));
		}

		public void Disconnect(bool sendNotice = true)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Invalid comparison between Unknown and I4
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Invalid comparison between Unknown and I4
			if (!Connected)
			{
				throw new InvalidOperationException("Not connected to the server.");
			}
			_Settings.Logger?.Invoke(Severity.Info, string.Concat(new string[5]
			{
				_Header,
				"disconnecting from ",
				_ServerIp,
				":",
				_ServerPort.ToString()
			}));
			if (Connected && sendNotice)
			{
				WatsonMessage watsonMessage = new WatsonMessage();
				watsonMessage.Status = MessageStatus.Shutdown;
				SendInternal(watsonMessage, 0L, null);
			}
			if (_TokenSource != null && !_TokenSource.get_IsCancellationRequested())
			{
				_TokenSource.Cancel();
				_TokenSource.Dispose();
			}
			if (_SslStream != null)
			{
				((Stream)_SslStream).Close();
			}
			if (_TcpStream != null)
			{
				((Stream)_TcpStream).Close();
			}
			if (_Client != null)
			{
				_Client.Close();
			}
			while (true)
			{
				global::System.Threading.Tasks.Task dataReceiver = _DataReceiver;
				if (dataReceiver != null && (int)dataReceiver.get_Status() == 3)
				{
					global::System.Threading.Tasks.Task.Delay(10).Wait();
					continue;
				}
				break;
			}
			while (true)
			{
				global::System.Threading.Tasks.Task idleServerMonitor = _IdleServerMonitor;
				if (idleServerMonitor == null || (int)idleServerMonitor.get_Status() != 3)
				{
					break;
				}
				global::System.Threading.Tasks.Task.Delay(10).Wait();
			}
			Connected = false;
			_Settings.Logger?.Invoke(Severity.Info, string.Concat(new string[5]
			{
				_Header,
				"disconnected from ",
				_ServerIp,
				":",
				_ServerPort.ToString()
			}));
		}

		public void Authenticate(string presharedKey)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			if (string.IsNullOrEmpty(presharedKey))
			{
				throw new ArgumentNullException("presharedKey");
			}
			if (presharedKey.get_Length() != 16)
			{
				throw new ArgumentException("Preshared key length must be 16 bytes.");
			}
			WatsonMessage watsonMessage = new WatsonMessage();
			watsonMessage.Status = MessageStatus.AuthRequested;
			watsonMessage.PresharedKey = Encoding.get_UTF8().GetBytes(presharedKey);
			SendInternal(watsonMessage, 0L, null);
		}

		public bool Send(string data, Dictionary<object, object> metadata = null)
		{
			if (string.IsNullOrEmpty(data))
			{
				return Send(new byte[0], metadata);
			}
			return Send(Encoding.get_UTF8().GetBytes(data), metadata);
		}

		public bool Send(byte[] data, Dictionary<object, object> metadata = null, int start = 0)
		{
			if (data == null)
			{
				data = new byte[0];
			}
			WatsonCommon.BytesToStream(data, start, out var contentLength, out var stream);
			return Send(contentLength, stream, metadata);
		}

		public bool Send(long contentLength, Stream stream, Dictionary<object, object> metadata = null)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			if (contentLength < 0)
			{
				throw new ArgumentException("Content length must be zero or greater.");
			}
			if (stream == null)
			{
				stream = (Stream)new MemoryStream(new byte[0]);
			}
			WatsonMessage msg = new WatsonMessage(metadata, contentLength, stream, syncRequest: false, syncResponse: false, null, null, _Settings.DebugMessages ? _Settings.Logger : null);
			return SendInternal(msg, contentLength, stream);
		}

		[AsyncStateMachine(typeof(<SendAsync>d__63))]
		public global::System.Threading.Tasks.Task<bool> SendAsync(string data, Dictionary<object, object> metadata = null, CancellationToken token = default(CancellationToken))
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			<SendAsync>d__63 <SendAsync>d__ = default(<SendAsync>d__63);
			<SendAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<SendAsync>d__.<>4__this = this;
			<SendAsync>d__.data = data;
			<SendAsync>d__.metadata = metadata;
			<SendAsync>d__.token = token;
			<SendAsync>d__.<>1__state = -1;
			<SendAsync>d__.<>t__builder.Start<<SendAsync>d__63>(ref <SendAsync>d__);
			return <SendAsync>d__.<>t__builder.get_Task();
		}

		[AsyncStateMachine(typeof(<SendAsync>d__64))]
		public global::System.Threading.Tasks.Task<bool> SendAsync(byte[] data, Dictionary<object, object> metadata = null, int start = 0, CancellationToken token = default(CancellationToken))
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			<SendAsync>d__64 <SendAsync>d__ = default(<SendAsync>d__64);
			<SendAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<SendAsync>d__.<>4__this = this;
			<SendAsync>d__.data = data;
			<SendAsync>d__.metadata = metadata;
			<SendAsync>d__.start = start;
			<SendAsync>d__.token = token;
			<SendAsync>d__.<>1__state = -1;
			<SendAsync>d__.<>t__builder.Start<<SendAsync>d__64>(ref <SendAsync>d__);
			return <SendAsync>d__.<>t__builder.get_Task();
		}

		[AsyncStateMachine(typeof(<SendAsync>d__65))]
		public global::System.Threading.Tasks.Task<bool> SendAsync(long contentLength, Stream stream, Dictionary<object, object> metadata = null, CancellationToken token = default(CancellationToken))
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			<SendAsync>d__65 <SendAsync>d__ = default(<SendAsync>d__65);
			<SendAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<SendAsync>d__.<>4__this = this;
			<SendAsync>d__.contentLength = contentLength;
			<SendAsync>d__.stream = stream;
			<SendAsync>d__.metadata = metadata;
			<SendAsync>d__.token = token;
			<SendAsync>d__.<>1__state = -1;
			<SendAsync>d__.<>t__builder.Start<<SendAsync>d__65>(ref <SendAsync>d__);
			return <SendAsync>d__.<>t__builder.get_Task();
		}

		public SyncResponse SendAndWait(int timeoutMs, string data, Dictionary<object, object> metadata = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			if (timeoutMs < 1000)
			{
				throw new ArgumentException("Timeout milliseconds must be 1000 or greater.");
			}
			if (string.IsNullOrEmpty(data))
			{
				return SendAndWait(timeoutMs, new byte[0], metadata);
			}
			return SendAndWait(timeoutMs, Encoding.get_UTF8().GetBytes(data), metadata);
		}

		public SyncResponse SendAndWait(int timeoutMs, byte[] data, Dictionary<object, object> metadata = null, int start = 0)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			if (timeoutMs < 1000)
			{
				throw new ArgumentException("Timeout milliseconds must be 1000 or greater.");
			}
			if (data == null)
			{
				data = new byte[0];
			}
			global::System.DateTime.get_Now().AddMilliseconds((double)timeoutMs);
			WatsonCommon.BytesToStream(data, start, out var contentLength, out var stream);
			return SendAndWait(timeoutMs, contentLength, stream, metadata);
		}

		public SyncResponse SendAndWait(int timeoutMs, long contentLength, Stream stream, Dictionary<object, object> metadata = null)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			if (contentLength < 0)
			{
				throw new ArgumentException("Content length must be zero or greater.");
			}
			if (timeoutMs < 1000)
			{
				throw new ArgumentException("Timeout milliseconds must be 1000 or greater.");
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
			return SendAndWaitInternal(msg, timeoutMs, contentLength, stream);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				_Settings.Logger?.Invoke(Severity.Info, _Header + "disposing");
				if (Connected)
				{
					Disconnect();
				}
				if (_WriteLock != null)
				{
					_WriteLock.Dispose();
				}
				if (_ReadLock != null)
				{
					_ReadLock.Dispose();
				}
				_Settings = null;
				_Events = null;
				_Callbacks = null;
				_Statistics = null;
				_Keepalive = null;
				_SslConfiguration = null;
				_SourceIp = null;
				_ServerIp = null;
				_Client = null;
				_DataStream = null;
				_TcpStream = null;
				_SslStream = null;
				_SslCertificate = null;
				_SslCertificateCollection = null;
				_WriteLock = null;
				_ReadLock = null;
				_DataReceiver = null;
			}
		}

		private void EnableKeepalives()
		{
		}

		[AsyncStateMachine(typeof(<DataReceiver>d__71))]
		private global::System.Threading.Tasks.Task DataReceiver()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			<DataReceiver>d__71 <DataReceiver>d__ = default(<DataReceiver>d__71);
			<DataReceiver>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DataReceiver>d__.<>4__this = this;
			<DataReceiver>d__.<>1__state = -1;
			((AsyncTaskMethodBuilder)(ref <DataReceiver>d__.<>t__builder)).Start<<DataReceiver>d__71>(ref <DataReceiver>d__);
			return ((AsyncTaskMethodBuilder)(ref <DataReceiver>d__.<>t__builder)).get_Task();
		}

		private bool SendInternal(WatsonMessage msg, long contentLength, Stream stream)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			if (msg == null)
			{
				throw new ArgumentNullException("msg");
			}
			if (!Connected)
			{
				return false;
			}
			if (contentLength > 0 && (stream == null || !stream.get_CanRead()))
			{
				throw new ArgumentException("Cannot read from supplied stream.");
			}
			bool flag = false;
			if (_Client == null || !_Client.get_Connected())
			{
				flag = true;
				return false;
			}
			_WriteLock.Wait();
			try
			{
				SendHeaders(msg);
				SendDataStream(contentLength, stream);
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
				_Settings.Logger?.Invoke(Severity.Error, string.Concat(new string[8]
				{
					_Header,
					"failed to write message to ",
					_ServerIp,
					":",
					_ServerPort.ToString(),
					":",
					Environment.get_NewLine(),
					SerializationHelper.SerializeJson(ex, pretty: true)
				}));
				_Events.HandleExceptionEncountered(this, new ExceptionEventArgs(ex));
				flag = true;
				return false;
			}
			finally
			{
				_WriteLock.Release();
				if (flag)
				{
					Connected = false;
					Dispose();
				}
			}
		}

		[AsyncStateMachine(typeof(<SendInternalAsync>d__73))]
		private global::System.Threading.Tasks.Task<bool> SendInternalAsync(WatsonMessage msg, long contentLength, Stream stream, CancellationToken token)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			<SendInternalAsync>d__73 <SendInternalAsync>d__ = default(<SendInternalAsync>d__73);
			<SendInternalAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<SendInternalAsync>d__.<>4__this = this;
			<SendInternalAsync>d__.msg = msg;
			<SendInternalAsync>d__.contentLength = contentLength;
			<SendInternalAsync>d__.stream = stream;
			<SendInternalAsync>d__.token = token;
			<SendInternalAsync>d__.<>1__state = -1;
			<SendInternalAsync>d__.<>t__builder.Start<<SendInternalAsync>d__73>(ref <SendInternalAsync>d__);
			return <SendInternalAsync>d__.<>t__builder.get_Task();
		}

		private SyncResponse SendAndWaitInternal(WatsonMessage msg, int timeoutMs, long contentLength, Stream stream)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Expected O, but got Unknown
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			<>c__DisplayClass74_0 CS$<>8__locals0 = new <>c__DisplayClass74_0();
			CS$<>8__locals0.msg = msg;
			if (CS$<>8__locals0.msg == null)
			{
				throw new ArgumentNullException("msg");
			}
			if (!Connected)
			{
				throw new InvalidOperationException("Client is not connected to the server.");
			}
			if (contentLength > 0 && (stream == null || !stream.get_CanRead()))
			{
				throw new ArgumentException("Cannot read from supplied stream.");
			}
			bool flag = false;
			if (_Client == null || !_Client.get_Connected())
			{
				flag = true;
				throw new InvalidOperationException("Client is not connected to the server.");
			}
			_WriteLock.Wait();
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
				SendHeaders(CS$<>8__locals0.msg);
				SendDataStream(contentLength, stream);
				_Statistics.IncrementSentMessages();
				_Statistics.AddSentBytes(contentLength);
			}
			catch (TaskCanceledException)
			{
				return null;
			}
			catch (OperationCanceledException)
			{
				return null;
			}
			catch (global::System.Exception ex)
			{
				_Settings.Logger?.Invoke(Severity.Error, string.Concat(new string[9]
				{
					_Header,
					"failed to write message to ",
					_ServerIp,
					":",
					_ServerPort.ToString(),
					":",
					Environment.get_NewLine(),
					((object)ex).ToString(),
					Environment.get_NewLine()
				}));
				flag = true;
				throw;
			}
			finally
			{
				_WriteLock.Release();
				if (flag)
				{
					Connected = false;
					Dispose();
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

		private void SendHeaders(WatsonMessage msg)
		{
			byte[] headerBytes = msg.HeaderBytes;
			_DataStream.Write(headerBytes, 0, headerBytes.Length);
			_DataStream.Flush();
		}

		[AsyncStateMachine(typeof(<SendHeadersAsync>d__76))]
		private global::System.Threading.Tasks.Task SendHeadersAsync(WatsonMessage msg, CancellationToken token)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			<SendHeadersAsync>d__76 <SendHeadersAsync>d__ = default(<SendHeadersAsync>d__76);
			<SendHeadersAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SendHeadersAsync>d__.<>4__this = this;
			<SendHeadersAsync>d__.msg = msg;
			<SendHeadersAsync>d__.token = token;
			<SendHeadersAsync>d__.<>1__state = -1;
			((AsyncTaskMethodBuilder)(ref <SendHeadersAsync>d__.<>t__builder)).Start<<SendHeadersAsync>d__76>(ref <SendHeadersAsync>d__);
			return ((AsyncTaskMethodBuilder)(ref <SendHeadersAsync>d__.<>t__builder)).get_Task();
		}

		private void SendDataStream(long contentLength, Stream stream)
		{
			if (contentLength <= 0)
			{
				return;
			}
			long num = contentLength;
			int num2 = 0;
			if (_Settings.StreamBufferSize != _SendBuffer.Length)
			{
				_SendBuffer = new byte[_Settings.StreamBufferSize];
			}
			while (num > 0)
			{
				num2 = stream.Read(_SendBuffer, 0, _SendBuffer.Length);
				if (num2 > 0)
				{
					_DataStream.Write(_SendBuffer, 0, num2);
					num -= num2;
				}
			}
			_DataStream.Flush();
		}

		[AsyncStateMachine(typeof(<SendDataStreamAsync>d__78))]
		private global::System.Threading.Tasks.Task SendDataStreamAsync(long contentLength, Stream stream, CancellationToken token)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			<SendDataStreamAsync>d__78 <SendDataStreamAsync>d__ = default(<SendDataStreamAsync>d__78);
			<SendDataStreamAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SendDataStreamAsync>d__.<>4__this = this;
			<SendDataStreamAsync>d__.contentLength = contentLength;
			<SendDataStreamAsync>d__.stream = stream;
			<SendDataStreamAsync>d__.token = token;
			<SendDataStreamAsync>d__.<>1__state = -1;
			((AsyncTaskMethodBuilder)(ref <SendDataStreamAsync>d__.<>t__builder)).Start<<SendDataStreamAsync>d__78>(ref <SendDataStreamAsync>d__);
			return ((AsyncTaskMethodBuilder)(ref <SendDataStreamAsync>d__.<>t__builder)).get_Task();
		}

		[AsyncStateMachine(typeof(<IdleServerMonitor>d__79))]
		private global::System.Threading.Tasks.Task IdleServerMonitor()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			<IdleServerMonitor>d__79 <IdleServerMonitor>d__ = default(<IdleServerMonitor>d__79);
			<IdleServerMonitor>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<IdleServerMonitor>d__.<>4__this = this;
			<IdleServerMonitor>d__.<>1__state = -1;
			((AsyncTaskMethodBuilder)(ref <IdleServerMonitor>d__.<>t__builder)).Start<<IdleServerMonitor>d__79>(ref <IdleServerMonitor>d__);
			return ((AsyncTaskMethodBuilder)(ref <IdleServerMonitor>d__.<>t__builder)).get_Task();
		}
	}
}
