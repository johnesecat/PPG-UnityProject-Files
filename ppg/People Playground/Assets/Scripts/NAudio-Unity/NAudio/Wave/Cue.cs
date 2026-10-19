using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace NAudio.Wave
{
	public class Cue
	{
		public int Position
		{
			[CompilerGenerated]
			get
			{
				return <Position>k__BackingField;
			}
		}

		public string Label
		{
			[CompilerGenerated]
			get
			{
				return <Label>k__BackingField;
			}
		}

		public Cue(int position, string label)
		{
			<Position>k__BackingField = position;
			if (label == null)
			{
				label = "";
			}
			<Label>k__BackingField = Regex.Replace(label, "[^\\u0000-\\u00FF]", "");
		}
	}
}
