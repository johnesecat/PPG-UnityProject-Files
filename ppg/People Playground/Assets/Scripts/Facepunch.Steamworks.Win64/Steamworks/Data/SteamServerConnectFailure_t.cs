using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SteamServerConnectFailure_t : ICallbackData
	{
		internal Result Result;

		[MarshalAs(UnmanagedType.I1)]
		internal bool StillRetrying;

		public static int _datasize = Marshal.SizeOf(typeof(SteamServerConnectFailure_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.SteamServerConnectFailure;
	}
}
