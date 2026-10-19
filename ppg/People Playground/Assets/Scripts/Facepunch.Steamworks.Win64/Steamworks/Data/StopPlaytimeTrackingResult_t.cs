using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct StopPlaytimeTrackingResult_t : ICallbackData
	{
		internal Result Result;

		public static int _datasize = Marshal.SizeOf(typeof(StopPlaytimeTrackingResult_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.StopPlaytimeTrackingResult;
	}
}
