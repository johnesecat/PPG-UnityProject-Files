using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct RemoteStoragePublishedFileUnsubscribed_t : ICallbackData
	{
		internal PublishedFileId PublishedFileId;

		internal AppId AppID;

		public static int _datasize = Marshal.SizeOf(typeof(RemoteStoragePublishedFileUnsubscribed_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.RemoteStoragePublishedFileUnsubscribed;
	}
}
