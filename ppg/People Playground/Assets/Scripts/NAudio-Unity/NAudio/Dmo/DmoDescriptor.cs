using System;
using System.Runtime.CompilerServices;

namespace NAudio.Dmo
{
	public class DmoDescriptor
	{
		public string Name
		{
			[CompilerGenerated]
			get
			{
				return <Name>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<Name>k__BackingField = value;
			}
		}

		public Guid Clsid
		{
			[CompilerGenerated]
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return <Clsid>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				<Clsid>k__BackingField = value;
			}
		}

		public DmoDescriptor(string name, Guid clsid)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			Name = name;
			Clsid = clsid;
		}
	}
}
