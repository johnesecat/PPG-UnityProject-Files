using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct EncryptedAppTicketResponse_t : ICallbackData
	{
		internal Result Result;

		public static int _datasize = Marshal.SizeOf(typeof(EncryptedAppTicketResponse_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.EncryptedAppTicketResponse;
	}
}
