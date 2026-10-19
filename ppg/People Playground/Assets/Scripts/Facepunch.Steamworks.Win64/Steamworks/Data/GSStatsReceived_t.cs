using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct GSStatsReceived_t : ICallbackData
	{
		internal Result Result;

		internal ulong SteamIDUser;

		public static int _datasize = Marshal.SizeOf(typeof(GSStatsReceived_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.GSStatsReceived;
	}
}
