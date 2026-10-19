using System.Runtime.CompilerServices;

namespace NAudio.Dmo
{
	public class MediaObjectSizeInfo
	{
		public int Size
		{
			[CompilerGenerated]
			get
			{
				return <Size>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<Size>k__BackingField = value;
			}
		}

		public int MaxLookahead
		{
			[CompilerGenerated]
			get
			{
				return <MaxLookahead>k__BackingField;
			}
		}

		public int Alignment
		{
			[CompilerGenerated]
			get
			{
				return <Alignment>k__BackingField;
			}
		}

		public MediaObjectSizeInfo(int size, int maxLookahead, int alignment)
		{
			Size = size;
			<MaxLookahead>k__BackingField = maxLookahead;
			<Alignment>k__BackingField = alignment;
		}

		public override string ToString()
		{
			return $"Size: {Size}, Alignment {Alignment}, MaxLookahead {MaxLookahead}";
		}
	}
}
