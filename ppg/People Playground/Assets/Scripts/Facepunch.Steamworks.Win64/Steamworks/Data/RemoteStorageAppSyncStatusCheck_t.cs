using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct RemoteStorageAppSyncStatusCheck_t : ICallbackData
	{
		internal AppId AppID;

		internal Result Result;

		public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageAppSyncStatusCheck_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.RemoteStorageAppSyncStatusCheck;
	}
}
