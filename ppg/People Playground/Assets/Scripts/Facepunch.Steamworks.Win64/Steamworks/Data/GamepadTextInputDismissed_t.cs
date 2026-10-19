using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct GamepadTextInputDismissed_t : ICallbackData
	{
		[MarshalAs(UnmanagedType.I1)]
		internal bool Submitted;

		internal uint SubmittedText;

		public static int _datasize = Marshal.SizeOf(typeof(GamepadTextInputDismissed_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.GamepadTextInputDismissed;
	}
}
