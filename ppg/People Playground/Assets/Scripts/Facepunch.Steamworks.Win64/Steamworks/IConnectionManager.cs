using System;
using Steamworks.Data;

namespace Steamworks
{
	public interface IConnectionManager
	{
		void OnConnecting(ConnectionInfo info);

		void OnConnected(ConnectionInfo info);

		void OnDisconnected(ConnectionInfo info);

		void OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel);
	}
}
