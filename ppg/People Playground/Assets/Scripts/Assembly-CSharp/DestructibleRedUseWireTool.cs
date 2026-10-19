using UnityEngine;

public class DestructibleRedUseWireTool : DestructibleUseWireTool
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
