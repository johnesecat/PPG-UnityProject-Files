using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct ClientGameServerDeny_t : ICallbackData
	{
		internal uint AppID;

		internal uint GameServerIP;

		internal ushort GameServerPort;

		internal ushort Secure;

		internal uint Reason;

		public static int _datasize = Marshal.SizeOf(typeof(ClientGameServerDeny_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.ClientGameServerDeny;
	}
}
