using System;
using System.Runtime.CompilerServices;
using NAudio.Utils;

namespace NAudio.Dsp
{
	internal class SimpleCompressor : AttRelEnvelope
	{
		private double envdB;

		public double MakeUpGain
		{
			[CompilerGenerated]
			get
			{
				return <MakeUpGain>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<MakeUpGain>k__BackingField = value;
			}
		}

		public double Threshold
		{
			[CompilerGenerated]
			get
			{
				return <Threshold>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<Threshold>k__BackingField = value;
			}
		}

		public double Ratio
		{
			[CompilerGenerated]
			get
			{
				return <Ratio>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<Ratio>k__BackingField = value;
			}
		}

		public SimpleCompressor(double attackTime, double releaseTime, double sampleRate)
			: base(attackTime, releaseTime, sampleRate)
		{
			Threshold = 0.0;
			Ratio = 1.0;
			MakeUpGain = 0.0;
			envdB = 1E-25;
		}

		public SimpleCompressor()
			: base(10.0, 10.0, 44100.0)
		{
			Threshold = 0.0;
			Ratio = 1.0;
			MakeUpGain = 0.0;
			envdB = 1E-25;
		}

		public void InitRuntime()
		{
			envdB = 1E-25;
		}

		public void Process(ref double in1, ref double in2)
		{
			double num = Math.Abs(in1);
			double num2 = Math.Abs(in2);
			double num3 = Decibels.LinearToDecibels(Math.Max(num, num2) + 1E-25) - Threshold;
			if (num3 < 0.0)
			{
				num3 = 0.0;
			}
			num3 += 1E-25;
			Run(num3, ref envdB);
			num3 = envdB - 1E-25;
			double dB = num3 * (Ratio - 1.0);
			dB = Decibels.DecibelsToLinear(dB) * Decibels.DecibelsToLinear(MakeUpGain);
			in1 *= dB;
			in2 *= dB;
		}
	}
}
