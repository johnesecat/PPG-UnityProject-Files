using System;

namespace NAudio.Dmo
{
	internal struct DmoPartialMediaType
	{
		private Guid type;

		private Guid subtype;

		public Guid Type
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return type;
			}
			internal set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				type = value;
			}
		}

		public Guid Subtype
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return subtype;
			}
			internal set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				subtype = value;
			}
		}
	}
}
