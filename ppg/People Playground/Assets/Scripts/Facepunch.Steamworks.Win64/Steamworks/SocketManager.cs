using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks
{
	public class SocketManager
	{
		public List<Connection> Connecting = new List<Connection>();

		public List<Connection> Connected = new List<Connection>();

		internal HSteamNetPollGroup pollGroup;

		public ISocketManager Interface { get; set; }

		public Socket Socket { get; internal set; }

		public override string ToString()
		{
			return Socket.ToString();
		}

		internal void Initialize()
		{
			pollGroup = SteamNetworkingSockets.Internal.CreatePollGroup();
		}

		public bool Close()
		{
			if (SteamNetworkingSockets.Internal.IsValid)
			{
				SteamNetworkingSockets.Internal.DestroyPollGroup(pollGroup);
				Socket.Close();
			}
			pollGroup = 0u;
			Socket = 0u;
			return true;
		}

		public virtual void OnConnectionChanged(Connection connection, ConnectionInfo info)
		{
			switch (info.State)
			{
			case ConnectionState.Connecting:
				if (!Connecting.Contains(connection))
				{
					Connecting.Add(connection);
					OnConnecting(connection, info);
				}
				break;
			case ConnectionState.Connected:
				if (!Connected.Contains(connection))
				{
					Connecting.Remove(connection);
					Connected.Add(connection);
					OnConnected(connection, info);
				}
				break;
			case ConnectionState.None:
			case ConnectionState.ClosedByPeer:
			case ConnectionState.ProblemDetectedLocally:
				if (Connecting.Contains(connection) || Connected.Contains(connection))
				{
					OnDisconnected(connection, info);
				}
				break;
			case ConnectionState.FindingRoute:
				break;
			}
		}

		public virtual void OnConnecting(Connection connection, ConnectionInfo info)
		{
			if (Interface != null)
			{
				Interface.OnConnecting(connection, info);
			}
			else
			{
				connection.Accept();
			}
		}

		public virtual void OnConnected(Connection connection, ConnectionInfo info)
		{
			SteamNetworkingSockets.Internal.SetConnectionPollGroup(connection, pollGroup);
			Interface?.OnConnected(connection, info);
		}

		public virtual void OnDisconnected(Connection connection, ConnectionInfo info)
		{
			SteamNetworkingSockets.Internal.SetConnectionPollGroup(connection, 0u);
			connection.Close();
			Connecting.Remove(connection);
			Connected.Remove(connection);
			Interface?.OnDisconnected(connection, info);
		}

		public void Receive(int bufferSize = 32)
		{
			int num = 0;
			IntPtr intPtr = Marshal.AllocHGlobal(IntPtr.Size * bufferSize);
			try
			{
				num = SteamNetworkingSockets.Internal.ReceiveMessagesOnPollGroup(pollGroup, intPtr, bufferSize);
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
				OnMessage(netMsg.Connection, netMsg.Identity, netMsg.DataPtr, netMsg.DataSize, netMsg.RecvTime, netMsg.MessageNumber, netMsg.Channel);
			}
			finally
			{
				NetMsg.InternalRelease((NetMsg*)(void*)msgPtr);
			}
		}

		public virtual void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum, long recvTime, int channel)
		{
			Interface?.OnMessage(connection, identity, data, size, messageNum, recvTime, channel);
		}
	}
}
