using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct GameConnectedChatJoin_t : ICallbackData
	{
		internal ulong SteamIDClanChat;

		internal ulong SteamIDUser;

		public static int _datasize = Marshal.SizeOf(typeof(GameConnectedChatJoin_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.GameConnectedChatJoin;
	}
}
