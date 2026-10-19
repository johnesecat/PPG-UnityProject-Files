using System;
using System.Runtime.InteropServices;

namespace NAudio.Wave
{
	public sealed class WaveFormatCustomMarshaler : ICustomMarshaler
	{
		private static WaveFormatCustomMarshaler marshaler;

		public static ICustomMarshaler GetInstance(string cookie)
		{
			if (marshaler == null)
			{
				marshaler = new WaveFormatCustomMarshaler();
			}
			return (ICustomMarshaler)(object)marshaler;
		}

		public void CleanUpManagedData(object ManagedObj)
		{
		}

		public void CleanUpNativeData(global::System.IntPtr pNativeData)
		{
			Marshal.FreeHGlobal(pNativeData);
		}

		public int GetNativeDataSize()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			throw new NotImplementedException();
		}

		public global::System.IntPtr MarshalManagedToNative(object ManagedObj)
		{
			return WaveFormat.MarshalToPtr((WaveFormat)ManagedObj);
		}

		public object MarshalNativeToManaged(global::System.IntPtr pNativeData)
		{
			return WaveFormat.MarshalFromPtr(pNativeData);
		}
	}
}
