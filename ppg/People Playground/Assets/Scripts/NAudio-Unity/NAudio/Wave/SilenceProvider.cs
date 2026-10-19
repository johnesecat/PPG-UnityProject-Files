using System;
using System.Runtime.CompilerServices;

namespace NAudio.Wave
{
	public class SilenceProvider : IWaveProvider
	{
		public WaveFormat WaveFormat
		{
			[CompilerGenerated]
			get
			{
				return <WaveFormat>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<WaveFormat>k__BackingField = value;
			}
		}

		public SilenceProvider(WaveFormat wf)
		{
			WaveFormat = wf;
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			global::System.Array.Clear((global::System.Array)buffer, offset, count);
			return count;
		}
	}
}
