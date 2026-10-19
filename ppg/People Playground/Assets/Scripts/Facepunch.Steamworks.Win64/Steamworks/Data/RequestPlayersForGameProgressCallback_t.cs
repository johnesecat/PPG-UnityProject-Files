using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct RequestPlayersForGameProgressCallback_t : ICallbackData
	{
		internal Result Result;

		internal ulong LSearchID;

		public static int _datasize = Marshal.SizeOf(typeof(RequestPlayersForGameProgressCallback_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.RequestPlayersForGameProgressCallback;
	}
}
