using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct MusicPlayerWantsPlayingRepeatStatus_t : ICallbackData
	{
		internal int PlayingRepeatStatus;

		public static int _datasize = Marshal.SizeOf(typeof(MusicPlayerWantsPlayingRepeatStatus_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.MusicPlayerWantsPlayingRepeatStatus;
	}
}
