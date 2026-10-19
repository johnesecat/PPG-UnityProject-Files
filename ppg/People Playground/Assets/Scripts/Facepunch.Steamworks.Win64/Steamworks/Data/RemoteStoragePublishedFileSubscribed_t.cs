using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct RemoteStoragePublishedFileSubscribed_t : ICallbackData
	{
		internal PublishedFileId PublishedFileId;

		internal AppId AppID;

		public static int _datasize = Marshal.SizeOf(typeof(RemoteStoragePublishedFileSubscribed_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.RemoteStoragePublishedFileSubscribed;
	}
}
