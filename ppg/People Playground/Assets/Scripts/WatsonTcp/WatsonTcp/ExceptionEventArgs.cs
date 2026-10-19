using System;
using System.Runtime.CompilerServices;

namespace WatsonTcp
{
	public class ExceptionEventArgs
	{
		public global::System.Exception Exception
		{
			[CompilerGenerated]
			get
			{
				return <Exception>k__BackingField;
			}
		}

		public string Json
		{
			[CompilerGenerated]
			get
			{
				return <Json>k__BackingField;
			}
		}

		internal ExceptionEventArgs(global::System.Exception e)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			<Exception>k__BackingField = e;
			<Json>k__BackingField = SerializationHelper.SerializeJson(e, pretty: true);
		}
	}
}
