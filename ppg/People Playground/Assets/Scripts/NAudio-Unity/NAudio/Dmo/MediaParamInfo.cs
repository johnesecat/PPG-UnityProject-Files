using System.Runtime.InteropServices;

namespace NAudio.Dmo
{
	internal struct MediaParamInfo
	{
		public MediaParamType mpType;

		public MediaParamCurveType mopCaps;

		public float mpdMinValue;

		public float mpdMaxValue;

		public float mpdNeutralValue;

		[MarshalAs(23, SizeConst = 32)]
		public string szUnitText;

		[MarshalAs(23, SizeConst = 32)]
		public string szLabel;
	}
}
