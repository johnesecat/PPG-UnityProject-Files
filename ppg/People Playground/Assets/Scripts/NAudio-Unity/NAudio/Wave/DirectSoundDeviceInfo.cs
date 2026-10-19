using System;
using System.Runtime.CompilerServices;

namespace NAudio.Wave
{
	public class DirectSoundDeviceInfo
	{
		public Guid Guid
		{
			[CompilerGenerated]
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return <Guid>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				<Guid>k__BackingField = value;
			}
		}

		public string Description
		{
			[CompilerGenerated]
			get
			{
				return <Description>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<Description>k__BackingField = value;
			}
		}

		public string ModuleName
		{
			[CompilerGenerated]
			get
			{
				return <ModuleName>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<ModuleName>k__BackingField = value;
			}
		}
	}
}
