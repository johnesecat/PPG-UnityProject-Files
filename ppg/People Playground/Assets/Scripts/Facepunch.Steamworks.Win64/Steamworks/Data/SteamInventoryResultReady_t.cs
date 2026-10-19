using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SteamInventoryResultReady_t : ICallbackData
	{
		internal int Handle;

		internal Result Result;

		public static int _datasize = Marshal.SizeOf(typeof(SteamInventoryResultReady_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.SteamInventoryResultReady;
	}
}
