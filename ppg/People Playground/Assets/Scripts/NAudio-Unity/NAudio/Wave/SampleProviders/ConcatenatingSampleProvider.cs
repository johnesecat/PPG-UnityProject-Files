using System;
using System.Collections.Generic;
using System.Linq;

namespace NAudio.Wave.SampleProviders
{
	public class ConcatenatingSampleProvider : ISampleProvider
	{
		private readonly ISampleProvider[] providers;

		private int currentProviderIndex;

		public WaveFormat WaveFormat => providers[0].WaveFormat;

		public ConcatenatingSampleProvider(global::System.Collections.Generic.IEnumerable<ISampleProvider> providers)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			if (providers == null)
			{
				throw new ArgumentNullException("providers");
			}
			this.providers = Enumerable.ToArray<ISampleProvider>(providers);
			if (this.providers.Length == 0)
			{
				throw new ArgumentException("Must provide at least one input", "providers");
			}
			if (Enumerable.Any<ISampleProvider>((global::System.Collections.Generic.IEnumerable<ISampleProvider>)this.providers, (Func<ISampleProvider, bool>)((ISampleProvider p) => p.WaveFormat.Channels != WaveFormat.Channels)))
			{
				throw new ArgumentException("All inputs must have the same channel count", "providers");
			}
			if (Enumerable.Any<ISampleProvider>((global::System.Collections.Generic.IEnumerable<ISampleProvider>)this.providers, (Func<ISampleProvider, bool>)((ISampleProvider p) => p.WaveFormat.SampleRate != WaveFormat.SampleRate)))
			{
				throw new ArgumentException("All inputs must have the same sample rate", "providers");
			}
		}

		public int Read(float[] buffer, int offset, int count)
		{
			int num = 0;
			while (num < count && currentProviderIndex < providers.Length)
			{
				int count2 = count - num;
				int num2 = providers[currentProviderIndex].Read(buffer, num, count2);
				num += num2;
				if (num2 == 0)
				{
					currentProviderIndex++;
				}
			}
			return num;
		}
	}
}
