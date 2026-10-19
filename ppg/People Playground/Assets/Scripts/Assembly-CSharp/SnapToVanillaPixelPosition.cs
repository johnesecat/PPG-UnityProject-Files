using UnityEngine;

[ExecuteInEditMode]
public class SnapToVanillaPixelPosition : MonoBehaviour
{
	public float PixelSize = 0.028571429f;

	public bool Half;

	private float snap
	{
		get
		{
			if (!Half)
			{
				return PixelSize;
			}
			return PixelSize / 2f;
		}
	}
}
