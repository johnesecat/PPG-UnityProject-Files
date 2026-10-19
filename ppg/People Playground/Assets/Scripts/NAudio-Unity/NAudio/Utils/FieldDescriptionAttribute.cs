using System;
using System.Runtime.CompilerServices;

namespace NAudio.Utils
{
	[AttributeUsage(/*Could not decode attribute arguments.*/)]
	public class FieldDescriptionAttribute : global::System.Attribute
	{
		public string Description
		{
			[CompilerGenerated]
			get
			{
				return <Description>k__BackingField;
			}
		}

		public FieldDescriptionAttribute(string description)
		{
			<Description>k__BackingField = description;
		}

		public override string ToString()
		{
			return Description;
		}
	}
}
