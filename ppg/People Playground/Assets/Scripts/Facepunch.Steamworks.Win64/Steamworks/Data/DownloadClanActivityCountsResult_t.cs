using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct DownloadClanActivityCountsResult_t : ICallbackData
	{
		[MarshalAs(UnmanagedType.I1)]
		internal bool Success;

		public static int _datasize = Marshal.SizeOf(typeof(DownloadClanActivityCountsResult_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.DownloadClanActivityCountsResult;
	}
}
