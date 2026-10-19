using System.Collections.Generic;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct LiquidMixingController
{
	public static List<LiquidMixInstructions> MixInstructions;

	static LiquidMixingController()
	{
		MixInstructions = new List<LiquidMixInstructions>();
	}
}
