using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct UserStatsUnloaded_t : ICallbackData
	{
		internal ulong SteamIDUser;

		public static int _datasize = Marshal.SizeOf(typeof(UserStatsUnloaded_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.UserStatsUnloaded;
	}
}
