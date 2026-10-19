using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SteamServersDisconnected_t : ICallbackData
	{
		internal Result Result;

		public static int _datasize = Marshal.SizeOf(typeof(SteamServersDisconnected_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.SteamServersDisconnected;
	}
}
