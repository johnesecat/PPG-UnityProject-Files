using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks
{
	public class ConnectionManager
	{
		public Connection Connection;

		public bool Connected = false;

		public bool Connecting = true;

		public IConnectionManager Interface { get; set; }

		public ConnectionInfo ConnectionInfo { get; internal set; }

		public string ConnectionName
		{
			get
			{
				return Connection.ConnectionName;
			}
			set
			{
				Connection.ConnectionName = value;
			}
		}

		public long UserData
		{
			get
			{
				return Connection.UserData;
			}
			set
			{
				Connection.UserData = value;
			}
		}

		public void Close()
		{
			Connection.Close();
		}

		public override string ToString()
		{
			return Connection.ToString();
		}

		public virtual void OnConnectionChanged(ConnectionInfo info)
		{
			ConnectionInfo = info;
			switch (info.State)
			{
			case ConnectionState.Connecting:
				OnConnecting(info);
				break;
			case ConnectionState.Connected:
				OnConnected(info);
				break;
			case ConnectionState.None:
			case ConnectionState.ClosedByPeer:
			case ConnectionState.ProblemDetectedLocally:
				OnDisconnected(info);
				break;
			case ConnectionState.FindingRoute:
				break;
			}
		}

		public virtual void OnConnecting(ConnectionInfo info)
		{
			Interface?.OnConnecting(info);
			Connecting = true;
		}

		public virtual void OnConnected(ConnectionInfo info)
		{
			Interface?.OnConnected(info);
			Connected = true;
			Connecting = false;
		}

		public virtual void OnDisconnected(ConnectionInfo info)
		{
			Interface?.OnDisconnected(info);
			Connected = false;
			Connecting = false;
		}

		public void Receive(int bufferSize = 32)
		{
			int num = 0;
			IntPtr intPtr = Marshal.AllocHGlobal(IntPtr.Size * bufferSize);
			try
			{
				num = SteamNetworkingSockets.Internal.ReceiveMessagesOnConnection(Connection, intPtr, bufferSize);
				for (int i = 0; i < num; i++)
				{
					ReceiveMessage(Marshal.ReadIntPtr(intPtr, i * IntPtr.Size));
				}
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			if (num == bufferSize)
			{
				Receive(bufferSize);
			}
		}

		internal unsafe void ReceiveMessage(IntPtr msgPtr)
		{
			NetMsg netMsg = Marshal.PtrToStructure<NetMsg>(msgPtr);
			try
			{
				OnMessage(netMsg.DataPtr, netMsg.DataSize, netMsg.RecvTime, netMsg.MessageNumber, netMsg.Channel);
			}
			finally
			{
				NetMsg.InternalRelease((NetMsg*)(void*)msgPtr);
			}
		}

		public virtual void OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel)
		{
			Interface?.OnMessage(data, size, messageNum, recvTime, channel);
		}
	}
}
