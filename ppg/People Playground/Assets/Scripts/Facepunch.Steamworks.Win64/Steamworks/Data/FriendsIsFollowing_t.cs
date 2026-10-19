using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct FriendsIsFollowing_t : ICallbackData
	{
		internal Result Result;

		internal ulong SteamID;

		[MarshalAs(UnmanagedType.I1)]
		internal bool IsFollowing;

		public static int _datasize = Marshal.SizeOf(typeof(FriendsIsFollowing_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.FriendsIsFollowing;
	}
}
