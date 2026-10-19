using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct RemoteStorageUserVoteDetails_t : ICallbackData
	{
		internal Result Result;

		internal PublishedFileId PublishedFileId;

		internal WorkshopVote Vote;

		public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageUserVoteDetails_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.RemoteStorageUserVoteDetails;
	}
}
