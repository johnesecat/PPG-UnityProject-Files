using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct RemoteStorageFileWriteAsyncComplete_t : ICallbackData
	{
		internal Result Result;

		public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageFileWriteAsyncComplete_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.RemoteStorageFileWriteAsyncComplete;
	}
}
