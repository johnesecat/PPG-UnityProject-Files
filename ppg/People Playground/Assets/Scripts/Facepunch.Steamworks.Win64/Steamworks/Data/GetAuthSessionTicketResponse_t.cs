using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct GetAuthSessionTicketResponse_t : ICallbackData
	{
		internal uint AuthTicket;

		internal Result Result;

		public static int _datasize = Marshal.SizeOf(typeof(GetAuthSessionTicketResponse_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.GetAuthSessionTicketResponse;
	}
}
