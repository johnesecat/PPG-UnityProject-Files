using System;

namespace NAudio.CoreAudioApi
{
	public struct PropertyKey
	{
		public Guid formatId;

		public int propertyId;

		public PropertyKey(Guid formatId, int propertyId)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			this.formatId = formatId;
			this.propertyId = propertyId;
		}
	}
}
