using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct DeleteItemResult_t : ICallbackData
	{
		internal Result Result;

		internal PublishedFileId PublishedFileId;

		public static int _datasize = Marshal.SizeOf(typeof(DeleteItemResult_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.DeleteItemResult;
	}
}
