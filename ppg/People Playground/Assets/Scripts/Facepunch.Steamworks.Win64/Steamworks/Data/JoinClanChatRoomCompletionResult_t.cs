using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct JoinClanChatRoomCompletionResult_t : ICallbackData
	{
		internal ulong SteamIDClanChat;

		internal RoomEnter ChatRoomEnterResponse;

		public static int _datasize = Marshal.SizeOf(typeof(JoinClanChatRoomCompletionResult_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.JoinClanChatRoomCompletionResult;
	}
}
