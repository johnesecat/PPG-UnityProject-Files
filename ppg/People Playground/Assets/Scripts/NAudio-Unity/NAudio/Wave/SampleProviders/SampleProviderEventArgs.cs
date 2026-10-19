using System;
using System.Runtime.CompilerServices;

namespace NAudio.Wave.SampleProviders
{
	public class SampleProviderEventArgs : EventArgs
	{
		public ISampleProvider SampleProvider
		{
			[CompilerGenerated]
			get
			{
				return <SampleProvider>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<SampleProvider>k__BackingField = value;
			}
		}

		public SampleProviderEventArgs(ISampleProvider sampleProvider)
			: this()
		{
			SampleProvider = sampleProvider;
		}
	}
}
