using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LeaderboardFindResult_t : ICallbackData
	{
		internal ulong SteamLeaderboard;

		internal byte LeaderboardFound;

		public static int _datasize = Marshal.SizeOf(typeof(LeaderboardFindResult_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.LeaderboardFindResult;
	}
}
