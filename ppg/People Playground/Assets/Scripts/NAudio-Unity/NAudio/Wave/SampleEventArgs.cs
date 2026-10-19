using System;
using System.Runtime.CompilerServices;

namespace NAudio.Wave
{
	public class SampleEventArgs : EventArgs
	{
		public float Left
		{
			[CompilerGenerated]
			get
			{
				return <Left>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<Left>k__BackingField = value;
			}
		}

		public float Right
		{
			[CompilerGenerated]
			get
			{
				return <Right>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<Right>k__BackingField = value;
			}
		}

		public SampleEventArgs(float left, float right)
			: this()
		{
			Left = left;
			Right = right;
		}
	}
}
