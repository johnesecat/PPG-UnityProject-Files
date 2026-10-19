using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct GSClientApprove_t : ICallbackData
	{
		internal ulong SteamID;

		internal ulong OwnerSteamID;

		public static int _datasize = Marshal.SizeOf(typeof(GSClientApprove_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.GSClientApprove;
	}
}
