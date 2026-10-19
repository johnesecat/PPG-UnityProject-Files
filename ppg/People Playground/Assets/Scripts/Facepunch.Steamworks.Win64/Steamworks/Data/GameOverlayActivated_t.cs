using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct GameOverlayActivated_t : ICallbackData
	{
		internal byte Active;

		public static int _datasize = Marshal.SizeOf(typeof(GameOverlayActivated_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.GameOverlayActivated;
	}
}
