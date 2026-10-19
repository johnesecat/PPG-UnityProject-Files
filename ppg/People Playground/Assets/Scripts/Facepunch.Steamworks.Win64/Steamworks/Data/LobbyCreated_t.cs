using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LobbyCreated_t : ICallbackData
	{
		internal Result Result;

		internal ulong SteamIDLobby;

		public static int _datasize = Marshal.SizeOf(typeof(LobbyCreated_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.LobbyCreated;
	}
}
