using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace WatsonTcp
{
	internal class ClientMetadata : global::System.IDisposable
	{
		internal SemaphoreSlim WriteLock;

		internal SemaphoreSlim ReadLock;

		internal CancellationTokenSource TokenSource;

		internal CancellationToken Token;

		private TcpClient _TcpClient;

		private NetworkStream _NetworkStream;

		private SslStream _SslStream;

		private Stream _DataStream;

		private string _IpPort;

		internal TcpClient TcpClient => _TcpClient;

		internal NetworkStream NetworkStream
		{
			get
			{
				return _NetworkStream;
			}
			set
			{
				_NetworkStream = value;
				if (_NetworkStream != null)
				{
					_DataStream = (Stream)(object)_NetworkStream;
				}
			}
		}

		internal SslStream SslStream
		{
			get
			{
				return _SslStream;
			}
			set
			{
				_SslStream = value;
				if (_SslStream != null)
				{
					_DataStream = (Stream)(object)_SslStream;
				}
			}
		}

		internal Stream DataStream => _DataStream;

		internal string IpPort => _IpPort;

		internal byte[] SendBuffer
		{
			[CompilerGenerated]
			get
			{
				return <SendBuffer>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<SendBuffer>k__BackingField = value;
			}
		}

		internal global::System.Threading.Tasks.Task DataReceiver
		{
			[CompilerGenerated]
			get
			{
				return <DataReceiver>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<DataReceiver>k__BackingField = value;
			}
		}

		internal ClientMetadata(TcpClient tcp)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected O, but got Unknown
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Expected O, but got Unknown
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			<SendBuffer>k__BackingField = new byte[65536];
			WriteLock = new SemaphoreSlim(1, 1);
			ReadLock = new SemaphoreSlim(1, 1);
			TokenSource = new CancellationTokenSource();
			base..ctor();
			if (tcp == null)
			{
				throw new ArgumentNullException("tcp");
			}
			_TcpClient = tcp;
			_IpPort = ((object)tcp.get_Client().get_RemoteEndPoint()).ToString();
			NetworkStream = tcp.GetStream();
			Token = TokenSource.get_Token();
		}

		public void Dispose()
		{
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Invalid comparison between Unknown and I4
			if (TokenSource != null && !TokenSource.get_IsCancellationRequested())
			{
				TokenSource.Cancel();
				TokenSource.Dispose();
			}
			if (_SslStream != null)
			{
				((Stream)_SslStream).Close();
			}
			if (_NetworkStream != null)
			{
				((Stream)_NetworkStream).Close();
			}
			if (_TcpClient != null)
			{
				_TcpClient.Close();
				_TcpClient.Dispose();
			}
			while (true)
			{
				global::System.Threading.Tasks.Task dataReceiver = DataReceiver;
				if (dataReceiver != null && (int)dataReceiver.get_Status() == 3)
				{
					global::System.Threading.Tasks.Task.Delay(30).Wait();
					continue;
				}
				break;
			}
		}
	}
}
