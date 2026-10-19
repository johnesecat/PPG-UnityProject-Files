using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct FriendsGetFollowerCount_t : ICallbackData
	{
		internal Result Result;

		internal ulong SteamID;

		internal int Count;

		public static int _datasize = Marshal.SizeOf(typeof(FriendsGetFollowerCount_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.FriendsGetFollowerCount;
	}
}
