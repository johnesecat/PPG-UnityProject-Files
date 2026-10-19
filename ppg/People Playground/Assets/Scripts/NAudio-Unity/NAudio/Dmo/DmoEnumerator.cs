using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NAudio.Dmo
{
	public class DmoEnumerator
	{
		public static global::System.Collections.Generic.IEnumerable<DmoDescriptor> GetAudioEffectNames()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return GetDmos(DmoGuids.DMOCATEGORY_AUDIO_EFFECT);
		}

		public static global::System.Collections.Generic.IEnumerable<DmoDescriptor> GetAudioEncoderNames()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return GetDmos(DmoGuids.DMOCATEGORY_AUDIO_ENCODER);
		}

		public static global::System.Collections.Generic.IEnumerable<DmoDescriptor> GetAudioDecoderNames()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return GetDmos(DmoGuids.DMOCATEGORY_AUDIO_DECODER);
		}

		private static global::System.Collections.Generic.IEnumerable<DmoDescriptor> GetDmos(Guid category)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			Marshal.ThrowExceptionForHR(DmoInterop.DMOEnum(ref category, DmoEnumFlags.None, 0, null, 0, null, out var enumDmo));
			int itemsFetched;
			do
			{
				enumDmo.Next(1, out var clsid, out var name, out itemsFetched);
				if (itemsFetched == 1)
				{
					string name2 = Marshal.PtrToStringUni(name);
					Marshal.FreeCoTaskMem(name);
					yield return new DmoDescriptor(name2, clsid);
				}
			}
			while (itemsFetched > 0);
		}
	}
}
