using System;
using System.Runtime.CompilerServices;

namespace NAudio.Wave.SampleProviders
{
	public class StreamVolumeEventArgs : EventArgs
	{
		public float[] MaxSampleValues
		{
			[CompilerGenerated]
			get
			{
				return <MaxSampleValues>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<MaxSampleValues>k__BackingField = value;
			}
		}

		public StreamVolumeEventArgs()
			: this()
		{
		}
	}
}
