using UnityEngine;

public class RedUseWireTool : UseWireTool
{
	protected override Color GetWireColor()
	{
		return new Color(11f, 1f, 0f);
	}

	protected override ushort GetWireChannel()
	{
		return 1;
	}
}
