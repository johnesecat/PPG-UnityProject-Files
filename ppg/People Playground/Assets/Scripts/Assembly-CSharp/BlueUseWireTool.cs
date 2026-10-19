using UnityEngine;

public class BlueUseWireTool : UseWireTool
{
	protected override Color GetWireColor()
	{
		return new Color(0f, 1f, 11f);
	}

	protected override ushort GetWireChannel()
	{
		return 2;
	}
}
