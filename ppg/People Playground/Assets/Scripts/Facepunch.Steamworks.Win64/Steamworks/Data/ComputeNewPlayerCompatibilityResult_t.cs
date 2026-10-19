using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct ComputeNewPlayerCompatibilityResult_t : ICallbackData
	{
		internal Result Result;

		internal int CPlayersThatDontLikeCandidate;

		internal int CPlayersThatCandidateDoesntLike;

		internal int CClanPlayersThatDontLikeCandidate;

		internal ulong SteamIDCandidate;

		public static int _datasize = Marshal.SizeOf(typeof(ComputeNewPlayerCompatibilityResult_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.ComputeNewPlayerCompatibilityResult;
	}
}
