using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct GSStatsUnloaded_t : ICallbackData
	{
		internal ulong SteamIDUser;

		public static int _datasize = Marshal.SizeOf(typeof(GSStatsUnloaded_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.UserStatsUnloaded;
	}
}
