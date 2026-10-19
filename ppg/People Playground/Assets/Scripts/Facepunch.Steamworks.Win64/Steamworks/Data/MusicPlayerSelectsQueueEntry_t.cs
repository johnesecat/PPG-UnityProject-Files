using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct MusicPlayerSelectsQueueEntry_t : ICallbackData
	{
		internal int NID;

		public static int _datasize = Marshal.SizeOf(typeof(MusicPlayerSelectsQueueEntry_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.MusicPlayerSelectsQueueEntry;
	}
}
