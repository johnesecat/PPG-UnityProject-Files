using System;

namespace NAudio.CoreAudioApi.Interfaces
{
	public struct Blob
	{
		public int Length;

		public global::System.IntPtr Data;

		private void FixCS0649()
		{
			Length = 0;
			Data = global::System.IntPtr.Zero;
		}
	}
}
