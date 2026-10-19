using System;
using System.Runtime.InteropServices;

namespace NAudio.Wave.Compression
{
	internal class AcmInterop
	{
		public delegate bool AcmDriverEnumCallback(global::System.IntPtr hAcmDriverId, global::System.IntPtr instance, AcmDriverDetailsSupportFlags flags);

		public delegate bool AcmFormatEnumCallback(global::System.IntPtr hAcmDriverId, ref AcmFormatDetails formatDetails, global::System.IntPtr dwInstance, AcmDriverDetailsSupportFlags flags);

		public delegate bool AcmFormatTagEnumCallback(global::System.IntPtr hAcmDriverId, ref AcmFormatTagDetails formatTagDetails, global::System.IntPtr dwInstance, AcmDriverDetailsSupportFlags flags);

		public delegate bool AcmFormatChooseHookProc(global::System.IntPtr windowHandle, int message, global::System.IntPtr wParam, global::System.IntPtr lParam);

		[DllImport("msacm32.dll")]
		public static extern MmResult acmDriverAdd(out global::System.IntPtr driverHandle, global::System.IntPtr driverModule, global::System.IntPtr driverFunctionAddress, int priority, AcmDriverAddFlags flags);

		[DllImport("msacm32.dll")]
		public static extern MmResult acmDriverRemove(global::System.IntPtr driverHandle, int removeFlags);

		[DllImport("Msacm32.dll")]
		public static extern MmResult acmDriverClose(global::System.IntPtr hAcmDriver, int closeFlags);

		[DllImport("Msacm32.dll")]
		public static extern MmResult acmDriverEnum(AcmDriverEnumCallback fnCallback, global::System.IntPtr dwInstance, AcmDriverEnumFlags flags);

		[DllImport("Msacm32.dll")]
		public static extern MmResult acmDriverDetails(global::System.IntPtr hAcmDriver, ref AcmDriverDetails driverDetails, int reserved);

		[DllImport("Msacm32.dll")]
		public static extern MmResult acmDriverOpen(out global::System.IntPtr pAcmDriver, global::System.IntPtr hAcmDriverId, int openFlags);

		[DllImport("Msacm32.dll", EntryPoint = "acmFormatChooseW")]
		public static extern MmResult acmFormatChoose(ref AcmFormatChoose formatChoose);

		[DllImport("Msacm32.dll")]
		public static extern MmResult acmFormatEnum(global::System.IntPtr hAcmDriver, ref AcmFormatDetails formatDetails, AcmFormatEnumCallback callback, global::System.IntPtr instance, AcmFormatEnumFlags flags);

		[DllImport("Msacm32.dll")]
		public static extern MmResult acmFormatSuggest(global::System.IntPtr hAcmDriver, [In][MarshalAs(44, MarshalType = "NAudio.Wave.WaveFormatCustomMarshaler")] WaveFormat sourceFormat, [In][Out][MarshalAs(44, MarshalType = "NAudio.Wave.WaveFormatCustomMarshaler")] WaveFormat destFormat, int sizeDestFormat, AcmFormatSuggestFlags suggestFlags);

		[DllImport("Msacm32.dll", EntryPoint = "acmFormatSuggest")]
		public static extern MmResult acmFormatSuggest2(global::System.IntPtr hAcmDriver, global::System.IntPtr sourceFormatPointer, global::System.IntPtr destFormatPointer, int sizeDestFormat, AcmFormatSuggestFlags suggestFlags);

		[DllImport("Msacm32.dll")]
		public static extern MmResult acmFormatTagEnum(global::System.IntPtr hAcmDriver, ref AcmFormatTagDetails formatTagDetails, AcmFormatTagEnumCallback callback, global::System.IntPtr instance, int reserved);

		[DllImport("Msacm32.dll")]
		public static extern MmResult acmMetrics(global::System.IntPtr hAcmObject, AcmMetrics metric, out int output);

		[DllImport("Msacm32.dll")]
		public static extern MmResult acmStreamOpen(out global::System.IntPtr hAcmStream, global::System.IntPtr hAcmDriver, [In][MarshalAs(44, MarshalType = "NAudio.Wave.WaveFormatCustomMarshaler")] WaveFormat sourceFormat, [In][MarshalAs(44, MarshalType = "NAudio.Wave.WaveFormatCustomMarshaler")] WaveFormat destFormat, [In] WaveFilter waveFilter, global::System.IntPtr callback, global::System.IntPtr instance, AcmStreamOpenFlags openFlags);

		[DllImport("Msacm32.dll", EntryPoint = "acmStreamOpen")]
		public static extern MmResult acmStreamOpen2(out global::System.IntPtr hAcmStream, global::System.IntPtr hAcmDriver, global::System.IntPtr sourceFormatPointer, global::System.IntPtr destFormatPointer, [In] WaveFilter waveFilter, global::System.IntPtr callback, global::System.IntPtr instance, AcmStreamOpenFlags openFlags);

		[DllImport("Msacm32.dll")]
		public static extern MmResult acmStreamClose(global::System.IntPtr hAcmStream, int closeFlags);

		[DllImport("Msacm32.dll")]
		public static extern MmResult acmStreamConvert(global::System.IntPtr hAcmStream, [In][Out] AcmStreamHeaderStruct streamHeader, AcmStreamConvertFlags streamConvertFlags);

		[DllImport("Msacm32.dll")]
		public static extern MmResult acmStreamPrepareHeader(global::System.IntPtr hAcmStream, [In][Out] AcmStreamHeaderStruct streamHeader, int prepareFlags);

		[DllImport("Msacm32.dll")]
		public static extern MmResult acmStreamReset(global::System.IntPtr hAcmStream, int resetFlags);

		[DllImport("Msacm32.dll")]
		public static extern MmResult acmStreamSize(global::System.IntPtr hAcmStream, int inputBufferSize, out int outputBufferSize, AcmStreamSizeFlags flags);

		[DllImport("Msacm32.dll")]
		public static extern MmResult acmStreamUnprepareHeader(global::System.IntPtr hAcmStream, [In][Out] AcmStreamHeaderStruct streamHeader, int flags);
	}
}
