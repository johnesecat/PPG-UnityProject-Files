using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using NAudio.Utils;

namespace NAudio.CoreAudioApi.Interfaces
{
	[StructLayout(2)]
	public struct PropVariant
	{
		[FieldOffset(0)]
		public short vt;

		[FieldOffset(2)]
		public short wReserved1;

		[FieldOffset(4)]
		public short wReserved2;

		[FieldOffset(6)]
		public short wReserved3;

		[FieldOffset(8)]
		public sbyte cVal;

		[FieldOffset(8)]
		public byte bVal;

		[FieldOffset(8)]
		public short iVal;

		[FieldOffset(8)]
		public ushort uiVal;

		[FieldOffset(8)]
		public int lVal;

		[FieldOffset(8)]
		public uint ulVal;

		[FieldOffset(8)]
		public int intVal;

		[FieldOffset(8)]
		public uint uintVal;

		[FieldOffset(8)]
		public long hVal;

		[FieldOffset(8)]
		public long uhVal;

		[FieldOffset(8)]
		public float fltVal;

		[FieldOffset(8)]
		public double dblVal;

		[FieldOffset(8)]
		public short boolVal;

		[FieldOffset(8)]
		public int scode;

		[FieldOffset(8)]
		public FILETIME filetime;

		[FieldOffset(8)]
		public Blob blobVal;

		[FieldOffset(8)]
		public global::System.IntPtr pointerValue;

		public VarEnum DataType => (VarEnum)vt;

		public object Value
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Invalid comparison between Unknown and I4
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_000e: Invalid comparison between Unknown and I4
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Invalid comparison between Unknown and I4
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Invalid comparison between Unknown and I4
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Invalid comparison between Unknown and I4
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Expected I4, but got Unknown
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Invalid comparison between Unknown and I4
				//IL_0054: Unknown result type (might be due to invalid IL or missing references)
				//IL_0057: Invalid comparison between Unknown and I4
				//IL_0059: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Invalid comparison between Unknown and I4
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Invalid comparison between Unknown and I4
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Invalid comparison between Unknown and I4
				//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
				//IL_0110: Unknown result type (might be due to invalid IL or missing references)
				//IL_011b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0126: Unknown result type (might be due to invalid IL or missing references)
				VarEnum dataType = DataType;
				if ((int)dataType <= 22)
				{
					if ((int)dataType <= 3)
					{
						if ((int)dataType == 2)
						{
							return iVal;
						}
						if ((int)dataType == 3)
						{
							return lVal;
						}
					}
					else
					{
						if ((int)dataType == 11)
						{
							switch (boolVal)
							{
							case -1:
								return true;
							case 0:
								return false;
							default:
								throw new NotSupportedException("PropVariant VT_BOOL must be either -1 or 0");
							}
						}
						switch (dataType - 16)
						{
						case 0:
							return bVal;
						case 4:
							return hVal;
						case 6:
							return iVal;
						case 3:
							return ulVal;
						case 5:
							return uhVal;
						}
					}
				}
				else if ((int)dataType <= 65)
				{
					if ((int)dataType == 31)
					{
						return Marshal.PtrToStringUni(pointerValue);
					}
					if ((int)dataType == 65)
					{
						goto IL_00d5;
					}
				}
				else
				{
					if ((int)dataType == 72)
					{
						return MarshalHelpers.PtrToStructure<Guid>(pointerValue);
					}
					if ((int)dataType == 4113)
					{
						goto IL_00d5;
					}
				}
				throw new NotImplementedException(string.Concat((object)"PropVariant ", (object)dataType));
				IL_00d5:
				return GetBlob();
			}
		}

		public static PropVariant FromLong(long value)
		{
			PropVariant result = default(PropVariant);
			result.vt = 20;
			result.hVal = value;
			return result;
		}

		private byte[] GetBlob()
		{
			byte[] array = new byte[blobVal.Length];
			Marshal.Copy(blobVal.Data, array, 0, array.Length);
			return array;
		}

		public T[] GetBlobAsArrayOf<T>()
		{
			int length = blobVal.Length;
			int num = Marshal.SizeOf((object)(T)Activator.CreateInstance(typeof(T)));
			if (length % num != 0)
			{
				throw new InvalidDataException($"Blob size {length} not a multiple of struct size {num}");
			}
			int num2 = length / num;
			T[] array = new T[num2];
			for (int i = 0; i < num2; i++)
			{
				array[i] = (T)Activator.CreateInstance(typeof(T));
				Marshal.PtrToStructure(new global::System.IntPtr((long)blobVal.Data + i * num), (object)array[i]);
			}
			return array;
		}

		[Obsolete("Call with pointer instead")]
		public void Clear()
		{
			PropVariantNative.PropVariantClear(ref this);
		}

		public static void Clear(global::System.IntPtr ptr)
		{
			PropVariantNative.PropVariantClear(ptr);
		}
	}
}
