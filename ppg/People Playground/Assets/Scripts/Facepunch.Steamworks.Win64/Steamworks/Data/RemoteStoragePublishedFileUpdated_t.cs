using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct RemoteStoragePublishedFileUpdated_t : ICallbackData
	{
		internal PublishedFileId PublishedFileId;

		internal AppId AppID;

		internal ulong Unused;

		public static int _datasize = Marshal.SizeOf(typeof(RemoteStoragePublishedFileUpdated_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.RemoteStoragePublishedFileUpdated;
	}
}
