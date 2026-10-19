using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct RequestPlayersForGameFinalResultCallback_t : ICallbackData
	{
		internal Result Result;

		internal ulong LSearchID;

		internal ulong LUniqueGameID;

		public static int _datasize = Marshal.SizeOf(typeof(RequestPlayersForGameFinalResultCallback_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.RequestPlayersForGameFinalResultCallback;
	}
}
