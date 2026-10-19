using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LeaderboardUGCSet_t : ICallbackData
	{
		internal Result Result;

		internal ulong SteamLeaderboard;

		public static int _datasize = Marshal.SizeOf(typeof(LeaderboardUGCSet_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.LeaderboardUGCSet;
	}
}
