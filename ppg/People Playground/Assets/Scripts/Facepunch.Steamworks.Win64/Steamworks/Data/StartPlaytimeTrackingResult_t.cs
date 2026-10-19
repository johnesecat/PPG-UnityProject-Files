using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct StartPlaytimeTrackingResult_t : ICallbackData
	{
		internal Result Result;

		public static int _datasize = Marshal.SizeOf(typeof(StartPlaytimeTrackingResult_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.StartPlaytimeTrackingResult;
	}
}
