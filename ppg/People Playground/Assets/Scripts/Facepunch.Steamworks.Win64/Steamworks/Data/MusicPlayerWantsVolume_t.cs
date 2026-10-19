using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct MusicPlayerWantsVolume_t : ICallbackData
	{
		internal float NewVolume;

		public static int _datasize = Marshal.SizeOf(typeof(MusicPlayerWantsVolume_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.MusicPlayerWantsVolume;
	}
}
