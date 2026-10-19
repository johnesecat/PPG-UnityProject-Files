using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct BroadcastUploadStart_t : ICallbackData
	{
		[MarshalAs(UnmanagedType.I1)]
		internal bool IsRTMP;

		public static int _datasize = Marshal.SizeOf(typeof(BroadcastUploadStart_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.BroadcastUploadStart;
	}
}
