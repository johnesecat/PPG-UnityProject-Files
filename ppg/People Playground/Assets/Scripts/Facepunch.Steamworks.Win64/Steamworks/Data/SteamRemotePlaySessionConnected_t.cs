using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SteamRemotePlaySessionConnected_t : ICallbackData
	{
		internal uint SessionID;

		public static int _datasize = Marshal.SizeOf(typeof(SteamRemotePlaySessionConnected_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.SteamRemotePlaySessionConnected;
	}
}
