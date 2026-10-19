using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct FriendRichPresenceUpdate_t : ICallbackData
	{
		internal ulong SteamIDFriend;

		internal AppId AppID;

		public static int _datasize = Marshal.SizeOf(typeof(FriendRichPresenceUpdate_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.FriendRichPresenceUpdate;
	}
}
