using System;
using NAudio.Utils;

namespace NAudio.Dsp
{
	internal class SimpleGate : AttRelEnvelope
	{
		private double threshdB;

		private double thresh;

		private double env;

		public double Threshold
		{
			get
			{
				return threshdB;
			}
			set
			{
				threshdB = value;
				thresh = Decibels.DecibelsToLinear(value);
			}
		}

		public SimpleGate()
			: base(10.0, 10.0, 44100.0)
		{
			threshdB = 0.0;
			thresh = 1.0;
			env = 1E-25;
		}

		public void Process(ref double in1, ref double in2)
		{
			double num = Math.Abs(in1);
			double num2 = Math.Abs(in2);
			double num3 = ((Math.Max(num, num2) > thresh) ? 1.0 : 0.0);
			num3 += 1E-25;
			Run(num3, ref env);
			num3 = env - 1E-25;
			in1 *= num3;
			in2 *= num3;
		}
	}
}
