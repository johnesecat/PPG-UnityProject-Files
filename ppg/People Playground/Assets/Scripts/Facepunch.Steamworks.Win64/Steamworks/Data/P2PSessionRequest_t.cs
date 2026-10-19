using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct P2PSessionRequest_t : ICallbackData
	{
		internal ulong SteamIDRemote;

		public static int _datasize = Marshal.SizeOf(typeof(P2PSessionRequest_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.P2PSessionRequest;
	}
}
