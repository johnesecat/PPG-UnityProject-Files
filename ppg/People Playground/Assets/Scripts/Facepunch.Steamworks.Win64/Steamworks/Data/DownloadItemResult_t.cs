using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct DownloadItemResult_t : ICallbackData
	{
		internal AppId AppID;

		internal PublishedFileId PublishedFileId;

		internal Result Result;

		public static int _datasize = Marshal.SizeOf(typeof(DownloadItemResult_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.DownloadItemResult;
	}
}
