using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

namespace NAudio.Wave.Asio
{
	public class AsioDriver
	{
		[StructLayout(0, Pack = 2)]
		private class AsioDriverVTable
		{
			[UnmanagedFunctionPointer(/*Could not decode attribute arguments.*/)]
			public delegate int ASIOInit(global::System.IntPtr _pUnknown, global::System.IntPtr sysHandle);

			[UnmanagedFunctionPointer(/*Could not decode attribute arguments.*/)]
			public delegate void ASIOgetDriverName(global::System.IntPtr _pUnknown, StringBuilder name);

			[UnmanagedFunctionPointer(/*Could not decode attribute arguments.*/)]
			public delegate int ASIOgetDriverVersion(global::System.IntPtr _pUnknown);

			[UnmanagedFunctionPointer(/*Could not decode attribute arguments.*/)]
			public delegate void ASIOgetErrorMessage(global::System.IntPtr _pUnknown, StringBuilder errorMessage);

			[UnmanagedFunctionPointer(/*Could not decode attribute arguments.*/)]
			public delegate AsioError ASIOstart(global::System.IntPtr _pUnknown);

			[UnmanagedFunctionPointer(/*Could not decode attribute arguments.*/)]
			public delegate AsioError ASIOstop(global::System.IntPtr _pUnknown);

			[UnmanagedFunctionPointer(/*Could not decode attribute arguments.*/)]
			public delegate AsioError ASIOgetChannels(global::System.IntPtr _pUnknown, out int numInputChannels, out int numOutputChannels);

			[UnmanagedFunctionPointer(/*Could not decode attribute arguments.*/)]
			public delegate AsioError ASIOgetLatencies(global::System.IntPtr _pUnknown, out int inputLatency, out int outputLatency);

			[UnmanagedFunctionPointer(/*Could not decode attribute arguments.*/)]
			public delegate AsioError ASIOgetBufferSize(global::System.IntPtr _pUnknown, out int minSize, out int maxSize, out int preferredSize, out int granularity);

			[UnmanagedFunctionPointer(/*Could not decode attribute arguments.*/)]
			public delegate AsioError ASIOcanSampleRate(global::System.IntPtr _pUnknown, double sampleRate);

			[UnmanagedFunctionPointer(/*Could not decode attribute arguments.*/)]
			public delegate AsioError ASIOgetSampleRate(global::System.IntPtr _pUnknown, out double sampleRate);

			[UnmanagedFunctionPointer(/*Could not decode attribute arguments.*/)]
			public delegate AsioError ASIOsetSampleRate(global::System.IntPtr _pUnknown, double sampleRate);

			[UnmanagedFunctionPointer(/*Could not decode attribute arguments.*/)]
			public delegate AsioError ASIOgetClockSources(global::System.IntPtr _pUnknown, out long clocks, int numSources);

			[UnmanagedFunctionPointer(/*Could not decode attribute arguments.*/)]
			public delegate AsioError ASIOsetClockSource(global::System.IntPtr _pUnknown, int reference);

			[UnmanagedFunctionPointer(/*Could not decode attribute arguments.*/)]
			public delegate AsioError ASIOgetSamplePosition(global::System.IntPtr _pUnknown, out long samplePos, ref Asio64Bit timeStamp);

			[UnmanagedFunctionPointer(/*Could not decode attribute arguments.*/)]
			public delegate AsioError ASIOgetChannelInfo(global::System.IntPtr _pUnknown, ref AsioChannelInfo info);

			[UnmanagedFunctionPointer(/*Could not decode attribute arguments.*/)]
			public delegate AsioError ASIOcreateBuffers(global::System.IntPtr _pUnknown, global::System.IntPtr bufferInfos, int numChannels, int bufferSize, global::System.IntPtr callbacks);

			[UnmanagedFunctionPointer(/*Could not decode attribute arguments.*/)]
			public delegate AsioError ASIOdisposeBuffers(global::System.IntPtr _pUnknown);

			[UnmanagedFunctionPointer(/*Could not decode attribute arguments.*/)]
			public delegate AsioError ASIOcontrolPanel(global::System.IntPtr _pUnknown);

			[UnmanagedFunctionPointer(/*Could not decode attribute arguments.*/)]
			public delegate AsioError ASIOfuture(global::System.IntPtr _pUnknown, int selector, global::System.IntPtr opt);

			[UnmanagedFunctionPointer(/*Could not decode attribute arguments.*/)]
			public delegate AsioError ASIOoutputReady(global::System.IntPtr _pUnknown);

			public ASIOInit init;

			public ASIOgetDriverName getDriverName;

			public ASIOgetDriverVersion getDriverVersion;

			public ASIOgetErrorMessage getErrorMessage;

			public ASIOstart start;

			public ASIOstop stop;

			public ASIOgetChannels getChannels;

			public ASIOgetLatencies getLatencies;

			public ASIOgetBufferSize getBufferSize;

			public ASIOcanSampleRate canSampleRate;

			public ASIOgetSampleRate getSampleRate;

			public ASIOsetSampleRate setSampleRate;

			public ASIOgetClockSources getClockSources;

			public ASIOsetClockSource setClockSource;

			public ASIOgetSamplePosition getSamplePosition;

			public ASIOgetChannelInfo getChannelInfo;

			public ASIOcreateBuffers createBuffers;

			public ASIOdisposeBuffers disposeBuffers;

			public ASIOcontrolPanel controlPanel;

			public ASIOfuture future;

			public ASIOoutputReady outputReady;
		}

		private global::System.IntPtr pAsioComObject;

		private global::System.IntPtr pinnedcallbacks;

		private AsioDriverVTable asioDriverVTable;

		private AsioDriver()
		{
		}

		public static string[] GetAsioDriverNames()
		{
			RegistryKey val = Registry.LocalMachine.OpenSubKey("SOFTWARE\\ASIO");
			string[] result = new string[0];
			if (val != null)
			{
				result = val.GetSubKeyNames();
				val.Close();
			}
			return result;
		}

		public static AsioDriver GetAsioDriverByName(string name)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			return GetAsioDriverByGuid(new Guid((Registry.LocalMachine.OpenSubKey("SOFTWARE\\ASIO\\" + name) ?? throw new ArgumentException($"Driver Name {name} doesn't exist")).GetValue("CLSID").ToString()));
		}

		public static AsioDriver GetAsioDriverByGuid(Guid guid)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			AsioDriver asioDriver = new AsioDriver();
			asioDriver.InitFromGuid(guid);
			return asioDriver;
		}

		public bool Init(global::System.IntPtr sysHandle)
		{
			return asioDriverVTable.init(pAsioComObject, sysHandle) == 1;
		}

		public string GetDriverName()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			StringBuilder val = new StringBuilder(256);
			asioDriverVTable.getDriverName(pAsioComObject, val);
			return ((object)val).ToString();
		}

		public int GetDriverVersion()
		{
			return asioDriverVTable.getDriverVersion(pAsioComObject);
		}

		public string GetErrorMessage()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			StringBuilder val = new StringBuilder(256);
			asioDriverVTable.getErrorMessage(pAsioComObject, val);
			return ((object)val).ToString();
		}

		public void Start()
		{
			HandleException(asioDriverVTable.start(pAsioComObject), "start");
		}

		public AsioError Stop()
		{
			return asioDriverVTable.stop(pAsioComObject);
		}

		public void GetChannels(out int numInputChannels, out int numOutputChannels)
		{
			HandleException(asioDriverVTable.getChannels(pAsioComObject, out numInputChannels, out numOutputChannels), "getChannels");
		}

		public AsioError GetLatencies(out int inputLatency, out int outputLatency)
		{
			return asioDriverVTable.getLatencies(pAsioComObject, out inputLatency, out outputLatency);
		}

		public void GetBufferSize(out int minSize, out int maxSize, out int preferredSize, out int granularity)
		{
			HandleException(asioDriverVTable.getBufferSize(pAsioComObject, out minSize, out maxSize, out preferredSize, out granularity), "getBufferSize");
		}

		public bool CanSampleRate(double sampleRate)
		{
			AsioError asioError = asioDriverVTable.canSampleRate(pAsioComObject, sampleRate);
			switch (asioError)
			{
			case AsioError.ASE_NoClock:
				return false;
			case AsioError.ASE_OK:
				return true;
			default:
				HandleException(asioError, "canSampleRate");
				return false;
			}
		}

		public double GetSampleRate()
		{
			HandleException(asioDriverVTable.getSampleRate(pAsioComObject, out var sampleRate), "getSampleRate");
			return sampleRate;
		}

		public void SetSampleRate(double sampleRate)
		{
			HandleException(asioDriverVTable.setSampleRate(pAsioComObject, sampleRate), "setSampleRate");
		}

		public void GetClockSources(out long clocks, int numSources)
		{
			HandleException(asioDriverVTable.getClockSources(pAsioComObject, out clocks, numSources), "getClockSources");
		}

		public void SetClockSource(int reference)
		{
			HandleException(asioDriverVTable.setClockSource(pAsioComObject, reference), "setClockSources");
		}

		public void GetSamplePosition(out long samplePos, ref Asio64Bit timeStamp)
		{
			HandleException(asioDriverVTable.getSamplePosition(pAsioComObject, out samplePos, ref timeStamp), "getSamplePosition");
		}

		public AsioChannelInfo GetChannelInfo(int channelNumber, bool trueForInputInfo)
		{
			AsioChannelInfo asioChannelInfo = default(AsioChannelInfo);
			asioChannelInfo.channel = channelNumber;
			asioChannelInfo.isInput = trueForInputInfo;
			AsioChannelInfo info = asioChannelInfo;
			HandleException(asioDriverVTable.getChannelInfo(pAsioComObject, ref info), "getChannelInfo");
			return info;
		}

		public void CreateBuffers(global::System.IntPtr bufferInfos, int numChannels, int bufferSize, ref AsioCallbacks callbacks)
		{
			pinnedcallbacks = Marshal.AllocHGlobal(Marshal.SizeOf((object)callbacks));
			Marshal.StructureToPtr((object)callbacks, pinnedcallbacks, false);
			HandleException(asioDriverVTable.createBuffers(pAsioComObject, bufferInfos, numChannels, bufferSize, pinnedcallbacks), "createBuffers");
		}

		public AsioError DisposeBuffers()
		{
			AsioError result = asioDriverVTable.disposeBuffers(pAsioComObject);
			Marshal.FreeHGlobal(pinnedcallbacks);
			return result;
		}

		public void ControlPanel()
		{
			HandleException(asioDriverVTable.controlPanel(pAsioComObject), "controlPanel");
		}

		public void Future(int selector, global::System.IntPtr opt)
		{
			HandleException(asioDriverVTable.future(pAsioComObject, selector, opt), "future");
		}

		public AsioError OutputReady()
		{
			return asioDriverVTable.outputReady(pAsioComObject);
		}

		public void ReleaseComAsioDriver()
		{
			Marshal.Release(pAsioComObject);
		}

		private void HandleException(AsioError error, string methodName)
		{
			if (error != 0 && error != AsioError.ASE_SUCCESS)
			{
				throw new AsioException($"Error code [{AsioException.getErrorName(error)}] while calling ASIO method <{methodName}>, {GetErrorMessage()}")
				{
					Error = error
				};
			}
		}

		private void InitFromGuid(Guid asioGuid)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			int num = CoCreateInstance(ref asioGuid, global::System.IntPtr.Zero, 1u, ref asioGuid, out pAsioComObject);
			if (num != 0)
			{
				throw new COMException("Unable to instantiate ASIO. Check if STAThread is set", num);
			}
			global::System.IntPtr intPtr = Marshal.ReadIntPtr(pAsioComObject);
			asioDriverVTable = new AsioDriverVTable();
			FieldInfo[] fields = typeof(AsioDriverVTable).GetFields();
			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo val = fields[i];
				object delegateForFunctionPointer = Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(intPtr, (i + 3) * global::System.IntPtr.get_Size()), val.get_FieldType());
				val.SetValue((object)asioDriverVTable, delegateForFunctionPointer);
			}
		}

		[DllImport("ole32.Dll")]
		private static extern int CoCreateInstance(ref Guid clsid, global::System.IntPtr inner, uint context, ref Guid uuid, out global::System.IntPtr rReturnedComObject);
	}
}
