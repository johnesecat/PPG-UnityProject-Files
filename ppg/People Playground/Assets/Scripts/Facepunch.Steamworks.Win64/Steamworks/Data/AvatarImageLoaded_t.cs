using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct AvatarImageLoaded_t : ICallbackData
	{
		internal ulong SteamID;

		internal int Image;

		internal int Wide;

		internal int Tall;

		public static int _datasize = Marshal.SizeOf(typeof(AvatarImageLoaded_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.AvatarImageLoaded;
	}
}
