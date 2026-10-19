using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct ItemInstalled_t : ICallbackData
	{
		internal AppId AppID;

		internal PublishedFileId PublishedFileId;

		public static int _datasize = Marshal.SizeOf(typeof(ItemInstalled_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.ItemInstalled;
	}
}
