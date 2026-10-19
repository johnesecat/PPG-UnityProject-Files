using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct GSGameplayStats_t : ICallbackData
	{
		internal Result Result;

		internal int Rank;

		internal uint TotalConnects;

		internal uint TotalMinutesPlayed;

		public static int _datasize = Marshal.SizeOf(typeof(GSGameplayStats_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.GSGameplayStats;
	}
}
