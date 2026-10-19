using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using NAudio.Utils;

namespace NAudio.Wave.Compression
{
	public class AcmDriver : global::System.IDisposable
	{
		private static List<AcmDriver> drivers;

		private AcmDriverDetails details;

		private global::System.IntPtr driverId;

		private global::System.IntPtr driverHandle;

		private List<AcmFormatTag> formatTags;

		private List<AcmFormat> tempFormatsList;

		private global::System.IntPtr localDllHandle;

		public int MaxFormatSize
		{
			get
			{
				MmException.Try(AcmInterop.acmMetrics(driverHandle, AcmMetrics.MaxSizeFormat, out var output), "acmMetrics");
				return output;
			}
		}

		public string ShortName => details.shortName;

		public string LongName => details.longName;

		public global::System.IntPtr DriverId => driverId;

		public global::System.Collections.Generic.IEnumerable<AcmFormatTag> FormatTags
		{
			get
			{
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				if (formatTags == null)
				{
					if (driverHandle == global::System.IntPtr.Zero)
					{
						throw new InvalidOperationException("Driver must be opened first");
					}
					formatTags = new List<AcmFormatTag>();
					AcmFormatTagDetails formatTagDetails = default(AcmFormatTagDetails);
					formatTagDetails.structureSize = Marshal.SizeOf((object)formatTagDetails);
					MmException.Try(AcmInterop.acmFormatTagEnum(driverHandle, ref formatTagDetails, AcmFormatTagEnumCallback, global::System.IntPtr.Zero, 0), "acmFormatTagEnum");
				}
				return (global::System.Collections.Generic.IEnumerable<AcmFormatTag>)formatTags;
			}
		}

		public static bool IsCodecInstalled(string shortName)
		{
			global::System.Collections.Generic.IEnumerator<AcmDriver> enumerator = EnumerateAcmDrivers().GetEnumerator();
			try
			{
				while (((global::System.Collections.IEnumerator)enumerator).MoveNext())
				{
					if (enumerator.get_Current().ShortName == shortName)
					{
						return true;
					}
				}
			}
			finally
			{
				((global::System.IDisposable)enumerator)?.Dispose();
			}
			return false;
		}

		public static AcmDriver AddLocalDriver(string driverFile)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			global::System.IntPtr intPtr = NativeMethods.LoadLibrary(driverFile);
			if (intPtr == global::System.IntPtr.Zero)
			{
				throw new ArgumentException("Failed to load driver file");
			}
			global::System.IntPtr procAddress = NativeMethods.GetProcAddress(intPtr, "DriverProc");
			if (procAddress == global::System.IntPtr.Zero)
			{
				NativeMethods.FreeLibrary(intPtr);
				throw new ArgumentException("Failed to discover DriverProc");
			}
			global::System.IntPtr hAcmDriver;
			MmResult mmResult = AcmInterop.acmDriverAdd(out hAcmDriver, intPtr, procAddress, 0, AcmDriverAddFlags.Function);
			if (mmResult != 0)
			{
				NativeMethods.FreeLibrary(intPtr);
				throw new MmException(mmResult, "acmDriverAdd");
			}
			AcmDriver acmDriver = new AcmDriver(hAcmDriver);
			if (string.IsNullOrEmpty(acmDriver.details.longName))
			{
				acmDriver.details.longName = "Local driver: " + Path.GetFileName(driverFile);
				acmDriver.localDllHandle = intPtr;
			}
			return acmDriver;
		}

		public static void RemoveLocalDriver(AcmDriver localDriver)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			if (localDriver.localDllHandle == global::System.IntPtr.Zero)
			{
				throw new ArgumentException("Please pass in the AcmDriver returned by the AddLocalDriver method");
			}
			MmResult result = AcmInterop.acmDriverRemove(localDriver.driverId, 0);
			NativeMethods.FreeLibrary(localDriver.localDllHandle);
			MmException.Try(result, "acmDriverRemove");
		}

		public static bool ShowFormatChooseDialog(global::System.IntPtr ownerWindowHandle, string windowTitle, AcmFormatEnumFlags enumFlags, WaveFormat enumFormat, out WaveFormat selectedFormat, out string selectedFormatDescription, out string selectedFormatTagDescription)
		{
			AcmFormatChoose formatChoose = default(AcmFormatChoose);
			formatChoose.structureSize = Marshal.SizeOf((object)formatChoose);
			formatChoose.styleFlags = AcmFormatChooseStyleFlags.None;
			formatChoose.ownerWindowHandle = ownerWindowHandle;
			int num = 200;
			formatChoose.selectedWaveFormatPointer = Marshal.AllocHGlobal(num);
			formatChoose.selectedWaveFormatByteSize = num;
			formatChoose.title = windowTitle;
			formatChoose.name = null;
			formatChoose.formatEnumFlags = enumFlags;
			formatChoose.waveFormatEnumPointer = global::System.IntPtr.Zero;
			if (enumFormat != null)
			{
				global::System.IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf((object)enumFormat));
				Marshal.StructureToPtr((object)enumFormat, intPtr, false);
				formatChoose.waveFormatEnumPointer = intPtr;
			}
			formatChoose.instanceHandle = global::System.IntPtr.Zero;
			formatChoose.templateName = null;
			MmResult mmResult = AcmInterop.acmFormatChoose(ref formatChoose);
			selectedFormat = null;
			selectedFormatDescription = null;
			selectedFormatTagDescription = null;
			if (mmResult == MmResult.NoError)
			{
				selectedFormat = WaveFormat.MarshalFromPtr(formatChoose.selectedWaveFormatPointer);
				selectedFormatDescription = formatChoose.formatDescription;
				selectedFormatTagDescription = formatChoose.formatTagDescription;
			}
			Marshal.FreeHGlobal(formatChoose.waveFormatEnumPointer);
			Marshal.FreeHGlobal(formatChoose.selectedWaveFormatPointer);
			if (mmResult != MmResult.AcmCancelled && mmResult != 0)
			{
				throw new MmException(mmResult, "acmFormatChoose");
			}
			return mmResult == MmResult.NoError;
		}

		public static AcmDriver FindByShortName(string shortName)
		{
			global::System.Collections.Generic.IEnumerator<AcmDriver> enumerator = EnumerateAcmDrivers().GetEnumerator();
			try
			{
				while (((global::System.Collections.IEnumerator)enumerator).MoveNext())
				{
					AcmDriver current = enumerator.get_Current();
					if (current.ShortName == shortName)
					{
						return current;
					}
				}
			}
			finally
			{
				((global::System.IDisposable)enumerator)?.Dispose();
			}
			return null;
		}

		public static global::System.Collections.Generic.IEnumerable<AcmDriver> EnumerateAcmDrivers()
		{
			drivers = new List<AcmDriver>();
			MmException.Try(AcmInterop.acmDriverEnum(DriverEnumCallback, global::System.IntPtr.Zero, (AcmDriverEnumFlags)0), "acmDriverEnum");
			return (global::System.Collections.Generic.IEnumerable<AcmDriver>)drivers;
		}

		private static bool DriverEnumCallback(global::System.IntPtr hAcmDriver, global::System.IntPtr dwInstance, AcmDriverDetailsSupportFlags flags)
		{
			drivers.Add(new AcmDriver(hAcmDriver));
			return true;
		}

		private AcmDriver(global::System.IntPtr hAcmDriver)
		{
			driverId = hAcmDriver;
			details = default(AcmDriverDetails);
			details.structureSize = Marshal.SizeOf((object)details);
			MmException.Try(AcmInterop.acmDriverDetails(hAcmDriver, ref details, 0), "acmDriverDetails");
		}

		public override string ToString()
		{
			return LongName;
		}

		public global::System.Collections.Generic.IEnumerable<AcmFormat> GetFormats(AcmFormatTag formatTag)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			if (driverHandle == global::System.IntPtr.Zero)
			{
				throw new InvalidOperationException("Driver must be opened first");
			}
			tempFormatsList = new List<AcmFormat>();
			AcmFormatDetails formatDetails = default(AcmFormatDetails);
			formatDetails.structSize = Marshal.SizeOf((object)formatDetails);
			formatDetails.waveFormatByteSize = 1024;
			formatDetails.waveFormatPointer = Marshal.AllocHGlobal(formatDetails.waveFormatByteSize);
			formatDetails.formatTag = (int)formatTag.FormatTag;
			MmResult result = AcmInterop.acmFormatEnum(driverHandle, ref formatDetails, AcmFormatEnumCallback, global::System.IntPtr.Zero, AcmFormatEnumFlags.None);
			Marshal.FreeHGlobal(formatDetails.waveFormatPointer);
			MmException.Try(result, "acmFormatEnum");
			return (global::System.Collections.Generic.IEnumerable<AcmFormat>)tempFormatsList;
		}

		public void Open()
		{
			if (driverHandle == global::System.IntPtr.Zero)
			{
				MmException.Try(AcmInterop.acmDriverOpen(out driverHandle, DriverId, 0), "acmDriverOpen");
			}
		}

		public void Close()
		{
			if (driverHandle != global::System.IntPtr.Zero)
			{
				MmException.Try(AcmInterop.acmDriverClose(driverHandle, 0), "acmDriverClose");
				driverHandle = global::System.IntPtr.Zero;
			}
		}

		private bool AcmFormatTagEnumCallback(global::System.IntPtr hAcmDriverId, ref AcmFormatTagDetails formatTagDetails, global::System.IntPtr dwInstance, AcmDriverDetailsSupportFlags flags)
		{
			formatTags.Add(new AcmFormatTag(formatTagDetails));
			return true;
		}

		private bool AcmFormatEnumCallback(global::System.IntPtr hAcmDriverId, ref AcmFormatDetails formatDetails, global::System.IntPtr dwInstance, AcmDriverDetailsSupportFlags flags)
		{
			tempFormatsList.Add(new AcmFormat(formatDetails));
			return true;
		}

		public void Dispose()
		{
			if (driverHandle != global::System.IntPtr.Zero)
			{
				Close();
				GC.SuppressFinalize((object)this);
			}
		}
	}
}
