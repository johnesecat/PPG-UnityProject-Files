using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SetUserItemVoteResult_t : ICallbackData
	{
		internal PublishedFileId PublishedFileId;

		internal Result Result;

		[MarshalAs(UnmanagedType.I1)]
		internal bool VoteUp;

		public static int _datasize = Marshal.SizeOf(typeof(SetUserItemVoteResult_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.SetUserItemVoteResult;
	}
}
