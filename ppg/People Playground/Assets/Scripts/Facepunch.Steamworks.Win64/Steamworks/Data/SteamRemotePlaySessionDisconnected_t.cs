using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SteamRemotePlaySessionDisconnected_t : ICallbackData
	{
		internal uint SessionID;

		public static int _datasize = Marshal.SizeOf(typeof(SteamRemotePlaySessionDisconnected_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.SteamRemotePlaySessionDisconnected;
	}
}
